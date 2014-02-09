using IBatisNet.DataMapper.TypeHandlers;
using ProphetsWay.Utilities;

namespace ProphetsWay.MyBatisTools
{
	public abstract class EnumTypeHandler<T> : ITypeHandlerCallback
	{
		public void SetParameter(IParameterSetter setter, object parameter)
		{
			setter.Value = ((T) parameter).ToString();
		}

		public object GetResult(IResultGetter getter)
		{
			var output = getter.Value.ToString().GetValue<T>();
			return output;
		}

		public object ValueOf(string s)
		{
			return s;
		}

		public object NullValue
		{
			get { return default(T); }
		}
	}
}