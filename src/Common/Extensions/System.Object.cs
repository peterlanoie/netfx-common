// ReSharper disable CheckNamespace
namespace System
// ReSharper restore CheckNamespace
{
	public static class ObjectExt
	{
		/// <summary>
		/// Returns the object's ToString() result truncated to <paramref name="charLimit"/>.
		/// </summary>
		/// <param name="obj">Instance of <see cref="System.Object"/>.</param>
		/// <param name="charLimit">Number of characters at which the input will be truncated.</param>
		/// <param name="suffix">Optional suffix to append to the end if the result is truncated.</param>
		/// <returns></returns>
		public static string ToString(this Object obj, int charLimit, string suffix = null)
		{
			var raw = obj.ToString();
			if (charLimit > 0)
			{
				if(raw.Length > charLimit)
				{
					raw = string.Concat(raw.Substring(0, charLimit), suffix);
				}
			}
			return raw;
		}
	}
}
