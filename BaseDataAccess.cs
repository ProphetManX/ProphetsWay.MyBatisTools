using System;
using System.Collections.Generic;
using IBatisNet.DataMapper;

namespace ProphetsWay.MyBatisTools
{
	public abstract class BaseDataAccess<TGenericDDLItem> : BaseDataAccess, IBaseDataAccess<TGenericDDLItem> 
		where TGenericDDLItem : BaseDDLItemClass
	{
		public IList<T> GetAll<T>() where T : TGenericDDLItem, new()
		{
			var tType = typeof(T);
			var item = new T();
			var mtd = GetType().GetMethod("GetAll", new[] { tType });

			if (mtd == null)
				throw new Exception(string.Format("Unable to find a 'GetAll' method for the type [{0}] specified.", tType.Name));

			return mtd.Invoke(this, new object[] { item }) as IList<T>;
		}

		public TGenericDDLItem Get(TGenericDDLItem item)
		{
			var tType = item.GetType();
			var mtd = GetType().GetMethod("Get", new[] { tType });

			if (mtd == null)
				throw new Exception(string.Format("Unable to find a 'Get' method for the type [{0}] specified.", tType.Name));

			return mtd.Invoke(this, new object[] { item }) as TGenericDDLItem;
		}

		public int Update(TGenericDDLItem item)
		{
			var tType = item.GetType();
			var mtd = GetType().GetMethod("Update", new[] { tType });

			if (mtd == null)
				throw new Exception(string.Format("Unable to find a 'Update' method for the type [{0}] specified.", tType.Name));

			return (int)mtd.Invoke(this, new object[] { item });
		}

		public int Delete(TGenericDDLItem item)
		{
			var tType = item.GetType();
			var mtd = GetType().GetMethod("Delete", new[] { tType });

			if (mtd == null)
				throw new Exception(string.Format("Unable to find a 'Delete' method for the type [{0}] specified.", tType.Name));

			return (int)mtd.Invoke(this, new object[] { item });
		}

		public void Insert(TGenericDDLItem item)
		{
			var tType = item.GetType();
			var mtd = GetType().GetMethod("Insert", new[] { tType });

			if (mtd == null)
				throw new Exception(string.Format("Unable to find a 'Insert' method for the type [{0}] specified.", tType.Name));

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

		public T Get<T>(int id) where T : class, new()
		{
			var tType = typeof(T);
			var mtd = GetType().GetMethod("Get", new[] { tType });

			if (mtd == null)
				throw new Exception(string.Format("Unable to find a 'Get' method for the type [{0}] specified.", typeof(T).Name));

			var input = new T();
			var prop = tType.GetProperty(string.Format("{0}Id", tType.Name));

			if (prop == null)
				prop = tType.GetProperty("Id");

			prop.SetValue(input, id);

			return mtd.Invoke(this, new object[] { input }) as T;
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