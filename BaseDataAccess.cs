using System;
using System.Collections.Generic;
using System.Reflection;
using IBatisNet.DataMapper;
using ProphetsWay.BaseDataAccess;

namespace ProphetsWay.MyBatisTools
{
	public abstract class BaseDataAccess<TGenericDDLItem> : BaseDataAccess, IBaseDataAccess<TGenericDDLItem> 
		where TGenericDDLItem : BaseDDLEntity
	{
		public IList<T> GetAll<T>() where T : TGenericDDLItem, new()
		{
			var tType = typeof(T);
			var item = new T();
			var mtd = GetType().GetMethod("GetAll", new[] { tType });

			if (mtd == null)
				throw new Exception($"Unable to find a 'GetAll' method for the type [{tType.Name}] specified.");

			return mtd.Invoke(this, new object[] { item }) as IList<T>;
		}

		public TGenericDDLItem Get(TGenericDDLItem item)
		{
			var tType = item.GetType();
			var mtd = GetType().GetMethod("Get", new[] { tType });

			if (mtd == null)
				throw new Exception($"Unable to find a 'Get' method for the type [{tType.Name}] specified.");

			return mtd.Invoke(this, new object[] { item }) as TGenericDDLItem;
		}

		public int Update(TGenericDDLItem item)
		{
			var tType = item.GetType();
			var mtd = GetType().GetMethod("Update", new[] { tType });

			if (mtd == null)
				throw new Exception($"Unable to find a 'Update' method for the type [{tType.Name}] specified.");

			return (int)mtd.Invoke(this, new object[] { item });
		}

		public int Delete(TGenericDDLItem item)
		{
			var tType = item.GetType();
			var mtd = GetType().GetMethod("Delete", new[] { tType });

			if (mtd == null)
				throw new Exception($"Unable to find a 'Delete' method for the type [{tType.Name}] specified.");

			return (int)mtd.Invoke(this, new object[] { item });
		}

		public void Insert(TGenericDDLItem item)
		{
			var tType = item.GetType();
			var mtd = GetType().GetMethod("Insert", new[] { tType });

			if (mtd == null)
				throw new Exception($"Unable to find a 'Insert' method for the type [{tType.Name}] specified.");

			mtd.Invoke(this, new object[] { item });
		}
	}

	public abstract class BaseDataAccess : IBaseDataAccess
	{
		protected readonly ISqlMapper _mapper;

		protected BaseDataAccess()
		{
			_mapper = GetType().Assembly.GenerateMapper();
		}

		public virtual T Get<T>(int id) where T : BaseEntity,new()
		{
			return Get<T>((object) id);
		}

		public virtual T Get<T>(long id) where T : BaseEntity, new()
		{
			return Get<T>((object) id);
		}

		public virtual IList<T> GetAll<T>(T item) where T : BaseEntity, new()
		{
			var tType = typeof(T);
			var mtd = GetType().GetMethod("GetAll", new[] {tType});

			if (mtd == null)
				throw new Exception($"Unable to find a 'GetAll' method for the type [{typeof(T).Name}] specified.");

			var input = new T();

			return mtd.Invoke(this, new object[] {input}) as IList<T>;
		}

		private T Get<T>(object id) where T : BaseEntity, new()
		{
			var mtd = GetMethod<T>("Get");
			var input = new T();
			var tType = typeof(T);

			var prop = tType.GetProperty($"{tType.Name}Id") ?? tType.GetProperty("Id");

			if (prop == null)
				throw new Exception($"Unable to find the 'Id' field on this type of object:  {typeof (T).Name}");

			prop.SetValue(input, id);

			return mtd.Invoke(this, new object[] { input }) as T;
		}

		public int Update<T>(T entity) where T : BaseEntity, new()
		{
			var mtd = GetMethod<T>("Update");
			return (int)mtd.Invoke(this, new object[] {entity});
		}

		public void Insert<T>(T entity) where T : BaseEntity, new()
		{
			var mtd = GetMethod<T>("Insert");
			mtd.Invoke(this, new object[] {entity});
		}

		private MethodInfo GetMethod<T>(string methodName)
		{
			var tType = typeof(T);
			var mtd = GetType().GetMethod(methodName, new[] { tType });

			if (mtd == null)
				throw new Exception($"Unable to find a '{methodName}' method for the type [{typeof(T).Name}] specified.");

			return mtd;
		}

		public void TransactionStart()
		{
			_mapper.BeginTransaction();
		}

		public void TransactionCommit()
		{
			_mapper.CommitTransaction();
		}

		public void TransactionRollBack()
		{
			_mapper.RollBackTransaction();
		}
	}
}