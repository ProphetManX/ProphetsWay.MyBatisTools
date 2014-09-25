using IBatisNet.DataMapper.TypeHandlers;
using ProphetsWay.Utilities;

namespace ProphetsWay.MyBatisTools
{
	public abstract class EnumTypeHandler<T> : ITypeHandlerCallback
	{
		public abstract void SetParameter(IParameterSetter setter, object parameter);

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

	public abstract class EnumTypeAsStringHandler<T> : EnumTypeHandler<T>
	{
		public override void SetParameter(IParameterSetter setter, object parameter)
		{
			setter.Value = ((T)parameter).ToString();
		}
	}

	public abstract class EnumTypeAsIntHandler<T> : EnumTypeHandler<T>
	{
		public override void SetParameter(IParameterSetter setter, object parameter)
		{
			setter.Value = ((T)parameter).GetHashCode();
		}
	}
}