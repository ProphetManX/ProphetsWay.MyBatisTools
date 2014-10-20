using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using IBatisNet.DataMapper;

namespace ProphetsWay.MyBatisTools
{
	public abstract class BaseDao
	{
		protected ISqlMapper Mapper { get; set; }

		protected BaseDao(ISqlMapper mapper)
		{
			Mapper = mapper;
		}
	}

	public abstract class BaseDao<T> : BaseDao, IBaseDao<T>
	{
		protected BaseDao(ISqlMapper mapper, Semaphore queue = null)
			: base(mapper)
		{
			_gatedQueries = queue != null;
			_gate = queue;

			_typeName = typeof (T).Name;
			_dbSpecificStmtId = string.Empty;

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

		private readonly string _typeName;
		private readonly string _dbSpecificStmtId;
		private string _getStmtId;
		private readonly bool _gatedQueries;
		private readonly Semaphore _gate;

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
			return GatedQueryForObject(GetStmtId, item);
		}

		public virtual void Insert(T item)
		{
			GatedInsert(InsertStmtId, item);
		}

		public virtual int Update(T item)
		{
			return GatedUpdate(UpdateStmtId, item);
		}

		public virtual int Delete(T item)
		{
			return GatedDelete(DeleteStmtId, item);
		}

		protected IList<T> GatedQueryForList(string stmtId, object obj)
		{
			return GatedQueryForList<T>(stmtId, obj);
		}

		protected IList<T2> GatedQueryForList<T2>(string stmtId, object obj)
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

		protected int GatedDelete(string stmtId, object obj)
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

		protected T GatedQueryForObject(string stmtId, object obj)
		{
			return GatedQueryForObject<T>(stmtId, obj);
		}

		protected T2 GatedQueryForObject<T2>(string stmtId, object obj)
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

		protected int GatedUpdate(string stmtId, object obj)
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

		protected void GatedInsert(string stmtId, object obj)
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