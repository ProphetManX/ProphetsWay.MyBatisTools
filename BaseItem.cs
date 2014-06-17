namespace ProphetsWay.MyBatisTools
{
	public class BaseItem: IBaseItem
	{
		public long Id { get; set; }
	}

	public interface IBaseItem
	{
		long Id { get; set; }
	}
}