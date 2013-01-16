using System.Reflection;
using IBatisNet.DataMapper;

namespace ProphetsWay.MyBatisTools
{
	public abstract class BaseDao
	{
		protected ISqlMapper _mapper { get; set; }

		protected BaseDao(Assembly assembly) : this(assembly.GenerateMapper()) { }

		protected BaseDao(ISqlMapper mapper)
		{
			_mapper = mapper;
		}


		/*		
		 * commented out until there is a vaild use case
		protected BaseDao()
		{
			_mapper = Assembly.GetAssembly(GetType()).GenerateMapper();
		}
		//*/

	}
}
