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
		where T : IBaseItem
	{
		protected BaseDao(ISqlMapper mapper)
			: base(mapper)
		{
			_typeName = typeof (T).Name;
		}

		private readonly string _typeName;

		private string _getStmtId;

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
					_insertStmtId = string.Format("{0}.Insert{0}", _typeName);

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
			return Mapper.QueryForObject<T>(GetStmtId, item.Id);
		}

		public virtual void Insert(T item)
		{
			Mapper.Insert(InsertStmtId, item);
		}

		public virtual int Update(T item)
		{
			return Mapper.Update(UpdateStmtId, item);
		}

		public virtual int Delete(T item)
		{
			return Mapper.Delete(DeleteStmtId, item.Id);
		}
	}
}