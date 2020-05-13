using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Common.ComponentModel.DataAnnotations
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ContextProvidedAttributeBase : ValidationAttribute
	{
		private const string MISSING_VALUE_KEY_ERROR_MSG = "Mismatch between ContextProvidedRangeAttribute usage and IContextValueProvider.GetValue() implementation on validation context class. GetValue() is expected to return a value for key '{0}' but did not.";

		protected object GetValueFromContext(string key, ValidationContext validationContext, bool allowNullValues = false)
		{
			if(!(validationContext.ObjectInstance is IContextValueProvider))
			{
				throw new InvalidOperationException("ContextProvidedRangeAttribute can only be used on a property belonging to a class that implements IContextValueProvider.");
			}

			IContextValueProvider provider = (IContextValueProvider)validationContext.ObjectInstance;
			object value;
			value = provider.GetValue(key);
			if(!allowNullValues && value == null)
			{
				throw new InvalidOperationException(string.Format(MISSING_VALUE_KEY_ERROR_MSG, key));
			}
			return value;
		}

	}
}
