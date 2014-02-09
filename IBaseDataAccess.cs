namespace ProphetsWay.MyBatisTools
{
	public interface IBaseDataAccess
	{
		T Get<T>(long id) where T : BaseItem, new();
	}
}