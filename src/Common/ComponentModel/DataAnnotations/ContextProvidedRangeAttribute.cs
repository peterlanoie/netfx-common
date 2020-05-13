using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Common.ComponentModel.DataAnnotations
{
	public class ContextProvidedRangeAttribute : ContextProvidedAttributeBase
	{
		private const string MISSING_VALUE_KEY_ERROR_MSG = "Mismatch between ContextProvidedRangeAttribute usage and IContextValueProvider.GetValue() implementation on validation context class. GetValue() is expected to return a value for key '{0}' but did not.";

		/// <summary>
		/// </summary>
		/// <param name="providerMinKey">Specifies the provider key used to retrieve the range minimum value.</param>
		/// <param name="providerMaxKey">Specifies the provider key used to retrieve the range maximum value.</param>
		public ContextProvidedRangeAttribute(string providerMinKey, string providerMaxKey)
		{
			ProviderMinKey = providerMinKey;
			ProviderMaxKey = providerMaxKey;
		}

		/// <summary>
		/// Specifies the provider key used to retrieve the range minimum value.
		/// </summary>
		public string ProviderMinKey { get; set; }

		/// <summary>
		/// Specifies the provider key used to retrieve the range maximum value.
		/// </summary>
		public string ProviderMaxKey { get; set; }

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			object min, max;

			min = GetValueFromContext(ProviderMinKey, validationContext);
			max = GetValueFromContext(ProviderMaxKey, validationContext);

			RangeAttribute validator;

			if(min is int && max is int)
			{
				validator = new RangeAttribute((int)min, (int)max);
			}
			else if(min is double && max is double)
			{
				validator = new RangeAttribute((double)min, (double)max);
			}
			else
			{
				validator = new RangeAttribute(min.GetType(), min.ToString(), max.ToString());
			}

			if(validator.IsValid(value))
			{
				return ValidationResult.Success;
			}
			return new ValidationResult(validator.ErrorMessage);
		}
	}
}
