using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Common.ComponentModel.DataAnnotations
{
	/// <summary>
	/// Defines an attribute used to decorate an enumerator to provide one or more string values used for persisting
	/// an enum value instance in a data store.  This attribute is used by utility methods to map and convert the concrete enum
	/// values to and from the string values.  Enums stored as numeric values can be converted directly to the enum type.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public class StringStorageValueAttribute : Attribute
	{
		/// <summary>
		/// The value stored for the enumerator value
		/// </summary>
		public string StoreValue { get; set; }

		/// <summary>
		/// Whether or not this is the preferred value to store when multiple values are defined.
		/// </summary>
		public bool Preferred { get; set; }

		public StringStorageValueAttribute(string storeValue)
		{
			StoreValue = storeValue;
		}

		public StringStorageValueAttribute(string storeValue, bool preferred) : this(storeValue)
		{
			Preferred = preferred;
		}
	}

}
