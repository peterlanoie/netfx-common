using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ReSharper disable CheckNamespace
namespace System
// ReSharper restore CheckNamespace
{
	public static class GuidExt
	{
		/// <summary>
		/// Returns the first 8 characters of the Guid.
		/// </summary>
		/// <returns></returns>
		public static string FirstOctet(this System.Guid obj)
		{
			return obj.ToString().Substring(0, 8);
		}

		/// <summary>
		/// Creates a string list of Guids with each separated by the <paramref name="separator"/>.
		/// Ex. ([{00000000-0000-0000-0000-000000000000},{11111111-1111-1111-1111-111111111111}], "|") returns "00000000-0000-0000-0000-000000000000|11111111-1111-1111-1111-111111111111"
		/// </summary>
		/// <param name="guids">The enumeration of Guids</param>
		/// <param name="separator">The string by which each is separated.</param>
		/// <returns></returns>
		public static string ToStringifiedList(this IEnumerable<Guid> guids, string separator)
		{
			return ToStringifiedList(guids, separator, null);
		}

		/// <summary>
		/// Creates a string list of Guids with each surrounded by the <paramref name="bounder"/> and separated by the <paramref name="separator"/>
		/// Ex. ([{00000000-0000-0000-0000-000000000000},{11111111-1111-1111-1111-111111111111}], "|", "'") returns "'00000000-0000-0000-0000-000000000000'|'11111111-1111-1111-1111-111111111111'"
		/// </summary>
		/// <param name="guids">The enumeration of Guids</param>
		/// <param name="separator">The string by which each is separated.</param>
		/// <param name="bounder">The single boundary string to wrap each Guid in.</param>
		/// <returns></returns>
		public static string ToStringifiedList(this IEnumerable<Guid> guids, string separator, string bounder)
		{
			return ToStringifiedList(guids, separator, bounder, bounder);
		}

		/// <summary>
		/// Creates a string list of Guids with each surrounded by the <paramref name="leftBounder"/> and <paramref name="rightBounder"/> and separated by the <paramref name="separator"/>
		/// Ex. ([{00000000-0000-0000-0000-000000000000},{11111111-1111-1111-1111-111111111111}], "|", "[", "]") returns "[00000000-0000-0000-0000-000000000000]|[11111111-1111-1111-1111-111111111111]"
		/// </summary>
		/// <param name="guids">The enumeration of Guids</param>
		/// <param name="separator">The string by which each is separated.</param>
		/// <param name="leftBounder">The left boundary string in which to wrap each Guid.</param>
		/// <param name="rightBounder">The right boundary string in which to wrap each Guid.</param>
		/// <returns></returns>
		public static string ToStringifiedList(this IEnumerable<Guid> guids, string separator, string leftBounder, string rightBounder)
		{
			return string.Join(separator, guids.Select(x => string.Format("{0}{1}{2}", leftBounder, x, rightBounder)).ToArray());
		}
	}
}
