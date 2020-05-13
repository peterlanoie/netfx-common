using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ReSharper disable CheckNamespace
namespace System
// ReSharper restore CheckNamespace
{
	public static class NullableTExt
	{
		public static T IfNull<T>(this T? value, T defaultValue) where T : struct
		{
			return value.GetValueOrDefault(defaultValue);
		}
	}
}
