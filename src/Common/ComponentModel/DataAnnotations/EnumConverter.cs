using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Common.ComponentModel.DataAnnotations
{
	public class EnumConverter<T>
	{
		internal class Mapping<TMapType>
		{
			public TMapType EnumValue { get; set; }
			public string StoreValue { get; set; }
			public bool Preferred { get; set; }
			public Mapping(string storeValue, TMapType enumValue, bool preferred)
			{
				StoreValue = storeValue;
				EnumValue = enumValue;
				Preferred = preferred;
			}
		}

		// Internal list of enum value to storage value mappings.
		private List<Mapping<T>> Map { get; set; }
	
		// Hide this so an instance can't be created.
		private EnumConverter()
		{
		}

		/// <summary>
		/// Converts the provided <paramref name="value"/> to the matching Enum value, if found.
		/// If not found an exception is thrown.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public T FromStorageValue(string value)
		{
			value = value.Trim();
			var mappedValue = Map.SingleOrDefault(x => x.StoreValue.Equals(value, StringComparison.InvariantCultureIgnoreCase));
			if (mappedValue == null)
			{
				throw new InvalidOperationException(string.Format("The enum {0} does not have a member with a StringStorageValueAttribute matching the provided value '{1}'.", typeof(T), value));
			}
			return mappedValue.EnumValue;
		}

		/// <summary>
		/// Converts the provided <paramref name="value"/> to the matching Enum value, if found.
		/// If not found, the <paramref name="defaultValue"/> is returned.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="defaultValue">A default value.</param>
		/// <returns></returns>
		public T FromStorageValue(string value, T defaultValue)
		{
			value = value.Trim();
			var mappedValue = Map.SingleOrDefault(x => x.StoreValue.Equals(value, StringComparison.InvariantCultureIgnoreCase));
			return mappedValue == null ? defaultValue : mappedValue.EnumValue;
		}

		public string ToStorageValue(T value)
		{
			var mappings = Map.Where(x => x.EnumValue.Equals(value));
			if (mappings.Count() == 1)
			{
				return mappings.First().StoreValue;
			}
			return mappings.Single(x => x.Preferred).StoreValue;
		}

		// internal static list of converters for caching
		private static List<object> _converters = new List<object>();

		private static object _converterListLock = new object();

		/// <summary>
		/// Create a new converter for <typeparamref name="T"/>
		/// </summary>
		/// <returns></returns>
		public static EnumConverter<T> GetConverter()
		{
			lock(_converterListLock)
			{
				var converter = _converters.FirstOrDefault(x => x is EnumConverter<T>) as EnumConverter<T>;
				if (converter == null)
				{
					converter = new EnumConverter<T>();
					var enumType = typeof (T);

					converter.Map = new List<Mapping<T>>();
					foreach (var field in enumType.GetFields().Where(x => x.IsStatic))
					{
						var enumValue = (T) Enum.Parse(enumType, field.Name);
						var mappings = field
							.GetCustomAttributes(typeof (StringStorageValueAttribute), false)
							.Cast<StringStorageValueAttribute>()
							.Select(x => new Mapping<T>
							             	(x.StoreValue, enumValue, x.Preferred)
							);
						if (mappings.Count() > 1)
						{
							var preferredCount = mappings.Count(x => x.Preferred);
							if (preferredCount == 0)
							{
								throw new InvalidOperationException(string.Format("More than one storage string value is defined for the enum '{0}' value '{1}', but none are marked as the preferred value for storage.", enumType, field.Name));
							}
							if (preferredCount > 1)
							{
								throw new InvalidOperationException(string.Format("More than one storage string value is defined for the enum '{0}' value '{1}', but {2} are marked as the preferred value for storage. Only one is allowed.", enumType, field.Name, preferredCount));
							}
						}
						converter.Map.AddRange(mappings);
					}
					if (converter.Map.Count == 0)
					{
						throw new InvalidOperationException(string.Format("Enum '{0}' contains no items decorated with the StringStorageValue attribute.", enumType));
					}
					_converters.Add(converter);
				}
				return converter;
			}
		}

	}
}
