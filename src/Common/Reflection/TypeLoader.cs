using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Common.Reflection
{
	public class TypeLoader
	{
		public static T LoadTypeByName<T>(string assembly, string typename, params object[] constructorArgs)
		{
			T objTypedInstance;
			var objAssembly = Assembly.Load(assembly);

			var objUntypedInstance = objAssembly.CreateInstance(typename, true, BindingFlags.Default, null, constructorArgs, null, null);
			if(objUntypedInstance == null)
			{
				throw new InvalidOperationException(string.Format("Type '{0}' could not be created from assembly '{1}'", typename, assembly));
			}
			try
			{
				objTypedInstance = (T)objUntypedInstance;
			}
			catch(Exception ex)
			{
				throw new InvalidCastException(string.Format("Type specified as '{0}' must be convertable to type '{1}'.", typename, typeof(T).ToString()), ex);
			}

			return objTypedInstance;
		}
	}
}
