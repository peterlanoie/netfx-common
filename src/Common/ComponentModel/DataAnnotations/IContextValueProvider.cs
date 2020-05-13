using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.ComponentModel.DataAnnotations
{
	public interface IContextValueProvider
	{
		object GetValue(string valueKey);
	}
}
