using System.Collections.Generic;

namespace ProphetsWay.MyBatisTools
{
	public interface IBaseDataAccess<TGenericDDLItem> : IBaseDataAccess, IBaseDao<TGenericDDLItem>
		where TGenericDDLItem : BaseDDLItemClass
	{
		IList<T> GetAll<T>() where T : TGenericDDLItem, new();
	}

	public interface IBaseDataAccess
	{
		T Get<T>(long id) where T : class, new();

		T Get<T>(int id) where T : class, new();

		void TransactionStart();

		void TransactionCommit();

		void TransactionRollBack();
	}

	
}