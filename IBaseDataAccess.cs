namespace ProphetsWay.MyBatisTools
{
	public interface IBaseDataAccess
	{
		T Get<T>(long id) where T : class, IBaseItem, new();
	}
}