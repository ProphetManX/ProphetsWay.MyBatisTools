using System;
using System.Linq;

namespace ProphetsWay.MyBatisTools
{
	public interface IBaseDataAccess
	{
		T Get<T>(long id) where T : class, new();

		void TransactionStart();

		void TransactionCommit();

		void TransactionRollBack();
	}

	public abstract class BaseDataAccess : IBaseDataAccess
	{
		public abstract void TransactionStart();
		public abstract void TransactionCommit();
		public abstract void TransactionRollBack();

		public T Get<T>(long id) where T : class, new()
		{
			var tType = typeof(T);
			var mtd = GetType().GetMethod("Get", new[] { tType });

			if (mtd == null)
				throw new Exception(string.Format("Unable to find a 'Get' method for the type [{0}] specified.", typeof(T).Name));

			var input = new T();
			var prop = tType.GetProperties().Single(x => x.Name == string.Format("{0}Id", tType.Name));

			prop.SetValue(input, id);

			return mtd.Invoke(this, new[] { input }) as T;
		}
	}
}