using IBatisNet.DataMapper;

namespace ProphetsWay.MyBatisTools
{
	public abstract class BaseAuditDao : BaseDao
	{
		protected BaseAuditDao(ISqlMapper mapper, int? userId) : base(mapper, userId)
		{

		}

		public abstract void AuditChange(object modified);

		public abstract void AuditDelete(object deleted);
	}
}