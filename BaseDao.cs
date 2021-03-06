﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Threading;
using IBatisNet.DataMapper;
using ProphetsWay.BaseDataAccess;

namespace ProphetsWay.MyBatisTools
{
	public abstract class BaseDao
	{
		protected BaseDao(ISqlMapper mapper, int? userId)
		{
			Mapper = mapper;
			UserId = userId;
		}

		protected ISqlMapper Mapper { get; set; }

		protected int? UserId { get; set; }
	}

	public abstract class BaseDDLDao<T> : BaseDao<T>, IBaseDDLDao<T> 
		where T : BaseDDLEntity, new()
	{
		protected BaseDDLDao(ISqlMapper mapper, int? userId = null, Semaphore queue = null) : base(mapper, userId, queue)
		{
			
		}

		private string _getAllStmtId;

		protected string GetAllStmtId
		{
			get
			{
				if (string.IsNullOrEmpty(_getAllStmtId))
					_getAllStmtId = string.Format("{0}.GetAll{0}{1}", _typeName, _dbSpecificStmtId);

				return _getAllStmtId;
			}
		}

		public virtual IList<T> GetAll(T item)
		{
			try
			{
				return GatedQueryForList(GetAllStmtId);
			}
			catch (Exception ex)
			{
				throw HandleException("GetAll", item, ex);
			}
		}
	}

	public abstract class BaseGetAllDao<T> : BaseDao<T>, IBaseGetAllDao<T> where T : BaseEntity, new()
	{
		public BaseGetAllDao(ISqlMapper mapper, int? userId = null, Semaphore queue = null, BaseAuditDao auditor = null) : base(mapper, userId, queue, auditor)
		{

		}

		private string _getAllStmtId;
		protected string GetAllStmtId
		{
			get
			{
				if (string.IsNullOrEmpty(_getAllStmtId))
					_getAllStmtId = $"{_typeName}.GetAll";

				return _getAllStmtId;
			}
		}

		public virtual IList<T> GetAll(T item)
		{
			try
			{
				return GatedQueryForList(GetStmtId);
			}
			catch (Exception ex)
			{
				throw HandleException("GetAll", item, ex);
			}
		}

	}

	public abstract class BaseDao<T> : BaseDao, IBaseDao<T> where T : class, new()
	{
        protected BaseDao(ISqlMapper mapper, Semaphore queue = null, BaseAuditDao auditor = null) : this(mapper, null, queue, auditor) { }

		protected BaseDao(ISqlMapper mapper, int? userId = null, Semaphore queue = null, BaseAuditDao auditor = null)
			: base(mapper, userId)
		{
			_gatedQueries = queue != null;
			_gate = queue;

			_typeName = typeof (T).Name;
			_dbSpecificStmtId = string.Empty;

			_auditor = auditor;

			bool providerSpecific;
			var providerSpecificStr = ConfigurationManager.AppSettings["ProviderSpecificQueries"];

			if (bool.TryParse(providerSpecificStr, out providerSpecific) && providerSpecific)
			{
				var connStr = ConfigurationManager.ConnectionStrings["MyBatisDBConnection"];
				if (connStr != null)
				{
					var providerName = connStr.ProviderName;

					if (providerName.ToLower().StartsWith("sqlserver"))
						_dbSpecificStmtId = "_sqlserver";

					if (providerName.ToLower().StartsWith("sqlite"))
						_dbSpecificStmtId = "_sqlite";
				}
			}
		}

		protected readonly string _typeName;
		protected readonly string _dbSpecificStmtId;
		private string _getStmtId;
		private readonly bool _gatedQueries;
		private readonly Semaphore _gate;
		private readonly BaseAuditDao _auditor;

		protected Semaphore Gate
		{
			get { return _gate; }
		}

		protected bool UseGate
		{
			get { return _gatedQueries; }
		}

		protected string GetStmtId
		{
			get
			{
				if (string.IsNullOrEmpty(_getStmtId))
					_getStmtId = string.Format("{0}.Get{0}ById", _typeName);

				return _getStmtId;
			}
		}

		private string _insertStmtId;

		protected string InsertStmtId
		{
			get
			{
				if (string.IsNullOrEmpty(_insertStmtId))
					_insertStmtId = string.Format("{0}.Insert{0}{1}", _typeName, _dbSpecificStmtId);

				return _insertStmtId;
			}
		}

		private string _updateStmtId;

		protected string UpdateStmtId
		{
			get
			{
				if (string.IsNullOrEmpty(_updateStmtId))
					_updateStmtId = string.Format("{0}.Update{0}", _typeName);

				return _updateStmtId;
			}
		}

		private string _deleteStmtId;

		protected string DeleteStmtId
		{
			get
			{
				if (string.IsNullOrEmpty(_deleteStmtId))
					_deleteStmtId = string.Format("{0}.Delete{0}ById", _typeName);

				return _deleteStmtId;
			}
		}

		public virtual T Get(T item)
		{
			try
			{
				return GatedQueryForObject(GetStmtId, item);
			}
			catch (Exception ex)
			{
				throw HandleException("Get", item, ex);
			}
		}

		public virtual void Insert(T item)
		{
			try
			{
				//var param = new Dictionary<string, object>();

				//var t = typeof (T);
				//var props = t.GetProperties();
				//foreach (var prop in props)
				//	param.Add(prop.Name, prop.GetValue(item));

				//param.Add("_UserId", UserId);

				GatedInsert(InsertStmtId, item);
			}
			catch (Exception ex)
			{
				throw HandleException("Insert", item, ex);
			}
		}

		public virtual int Update(T item)
		{
			try
			{
				//dynamic dItem = item;
				//dItem._UserId = UserId;

				_auditor?.AuditChange(item);

				return GatedUpdate(UpdateStmtId, item);
			}
			catch (Exception ex)
			{
				throw HandleException("Update", item, ex);
			}
		}

		public virtual int Delete(T item)
		{
			try
			{
				//dynamic dItem = item;
				//dItem._UserId = UserId;

				_auditor?.AuditDelete(item);

				return GatedDelete(DeleteStmtId, item);
			}
			catch (Exception ex)
			{
				throw HandleException("Delete", item, ex);
			}
		}

		protected IList<T> GatedQueryForList(string stmtId, T obj = null)
		{
			try
			{
				return GatedQueryForList<T>(stmtId, obj);
			}
			catch (Exception ex)
			{
				throw HandleException("GetListing", obj, ex);
			}
		}

		#region Exception Handling 

		protected Exception HandleException(string actionType, object item, Exception ex)
		{
			if (item == null)
				return ex;

			var idProp = GetIdProperty(item);
			if (idProp == null)
				return ex;

			var id = idProp.GetValue(item);
			return new Exception(GetExceptionMessage(actionType, item, id), ex);
		}

		private string GetItemType(object item)
		{
			return item.GetType().Name;
		}

		private PropertyInfo GetIdProperty(object item)
		{
			return item.GetType().GetProperty(string.Format("{0}Id", GetItemType(item)));
		}

		private string GetExceptionMessage(string actionType, object item, object id)
		{
			return string.Format("Unable to '{0}' item of type [{1}] from the database with the id: {2}", actionType,
				GetItemType(item), id);
		}

		#endregion

		protected IList<T2> GatedQueryForList<T2>(string stmtId, object obj = null)
		{
			if (_gatedQueries)
				try
				{
					_gate.WaitOne();
					return Mapper.QueryForList<T2>(stmtId, obj);
				}
				finally
				{
					_gate.Release();
				}

			return Mapper.QueryForList<T2>(stmtId, obj);
		}

		protected int GatedDelete(string stmtId, object obj = null)
		{
			if (_gatedQueries)
				try
				{
					_gate.WaitOne();
					return Mapper.Delete(stmtId, obj);
				}
				finally
				{
					_gate.Release();
				}

			return Mapper.Delete(stmtId, obj);
		}

		protected T GatedQueryForObject(string stmtId, object obj = null)
		{
			return GatedQueryForObject<T>(stmtId, obj);
		}

		protected T2 GatedQueryForObject<T2>(string stmtId, object obj = null)
		{
			if (UseGate)
				try
				{
					Gate.WaitOne();
					return Mapper.QueryForObject<T2>(stmtId, obj);
				}
				finally
				{
					Gate.Release();
				}

			return Mapper.QueryForObject<T2>(stmtId, obj);
		}

		protected int GatedUpdate(string stmtId, object obj = null)
		{
			if (UseGate)
				try
				{
					Gate.WaitOne();
					return Mapper.Update(stmtId, obj);
				}
				finally
				{
					Gate.Release();
				}

			return Mapper.Update(stmtId, obj);
		}

		protected void GatedInsert(string stmtId, object obj = null)
		{
			if (_gatedQueries)
				try
				{
					_gate.WaitOne();
					Mapper.Insert(stmtId, obj);
					return;
				}
				finally
				{
					_gate.Release();
				}

			Mapper.Insert(stmtId, obj);
		}

	}
}