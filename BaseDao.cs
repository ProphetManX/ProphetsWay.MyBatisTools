using IBatisNet.DataMapper;

namespace ProphetsWay.MyBatisTools
{
	public abstract class BaseDao
	{
		protected ISqlMapper _mapper { get; set; }

		protected BaseDao(ISqlMapper mapper)
		{
			_mapper = mapper;
		}
	}

	public abstract class BaseDao<T> : BaseDao, IBaseDao<T>
		where T : BaseItem
	{
		protected BaseDao(ISqlMapper mapper)
			: base(mapper)
		{
			_typeName = typeof(T).Name;
		}

		private readonly string _typeName;

		private string getStmtId;
		protected string _getStmtId
		{
			get
			{
				if (string.IsNullOrEmpty(getStmtId))
					getStmtId = string.Format("{0}.Get{0}ById", _typeName);

				return getStmtId;
			}
		}

		private string insertStmtId;
		protected string _insertStmtId
		{
			get
			{
				if (string.IsNullOrEmpty(insertStmtId))
					insertStmtId = string.Format("{0}.Insert{0}", _typeName);

				return insertStmtId;
			}
		}

		private string updateStmtId;
		protected string _updateStmtId
		{
			get
			{
				if (string.IsNullOrEmpty(updateStmtId))
					updateStmtId = string.Format("{0}.Update{0}", _typeName);

				return updateStmtId;
			}
		}

		private string deleteStmtId;
		protected string _deleteStmtId
		{
			get
			{
				if (string.IsNullOrEmpty(deleteStmtId))
					deleteStmtId = string.Format("{0}.Delete{0}ById", _typeName);

				return deleteStmtId;
			}
		}

		public virtual T Get(T item)
		{
			return _mapper.QueryForObject<T>(_getStmtId, item.Id);
		}

		public virtual void Insert(T item)
		{
			_mapper.Insert(_insertStmtId, item);
		}

		public virtual int Update(T item)
		{
			return _mapper.Update(_updateStmtId, item);
		}

		public virtual int Delete(T item)
		{
			return _mapper.Delete(_deleteStmtId, item.Id);
		}
	}
}
