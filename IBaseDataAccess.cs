using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProphetsWay.MyBatisTools
{
	public interface IBaseDataAccess
	{
		T Get<T>(long id) where T : BaseItem, new();
	}
}
