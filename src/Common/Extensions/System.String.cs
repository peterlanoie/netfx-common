using System.Linq;

// ReSharper disable CheckNamespace
namespace System
// ReSharper restore CheckNamespace
{
	public static class StringExt
	{
		/// <summary>
		/// Returns the string reversed.
		/// </summary>
		/// <param name="obj">Instance of <see cref="System.String"/>.</param>
		/// <returns></returns>
		public static string Reverse(this String obj)
		{
			return new string(obj.ToCharArray().Reverse().ToArray());
		}

		/// <summary>
		/// Tests whether or not the string exists in the provided list.
		/// </summary>
		/// <param name="obj">Instance of <see cref="System.String"/>.</param>
		/// <param name="possibleMatches">List in which to look for a match.</param>
		/// <returns></returns>
		public static bool In(this String obj, params string[] possibleMatches)
		{
			return possibleMatches.Contains(obj);
		}

		/// <summary>
		/// Tests whether or not the string contains any of the <paramref name="possibleMatches"/>.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="possibleMatches">List in which to look for a match.</param>
		/// <returns></returns>
		public static bool ContainsAny(this String obj, params string[] possibleMatches)
		{
			return obj != null && possibleMatches.Any(obj.Contains);
		}

		/// <summary>
		/// Removes all the values in the <paramref name="find"/> set from the <paramref name="source"/> string.
		/// </summary>
		/// <param name="source">String from which to remove the found strings.</param>
		/// <param name="find">Strings to find and remove from the <paramref name="source"/>.</param>
		/// <returns></returns>
		public static string Remove(this String source, params string[] find)
		{
			return find.Aggregate(source, (current, s) => current.Replace(s, ""));
		}

		/// <summary>
		/// Replaces a series of find/replace pairs in the string. ([findString1, replaceString1, findString2, replaceString2...])
		/// </summary>
		/// <param name="source">String in which the pairs will be found and replaced.</param>
		/// <param name="findReplacePairs">Even number of strings to serve as the find and replace strings.</param>
		/// <returns></returns>
		public static string ReplaceMany(this String source, params string[] findReplacePairs)
		{
			var result = source;
			if(findReplacePairs.Length % 2 == 1)
			{
				throw new InvalidOperationException("replacePairs must contain an even number of elements to form complete find/replace match sets");
			}
			for(var i = 0; i < findReplacePairs.Length; i += 2)
			{
				result = result.Replace(findReplacePairs[i], findReplacePairs[i + 1]);
			}
			return result;
		}

		/// <summary>
		/// Takes the left x chars of the string.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="chars">Number of chars to take. A string containing less will return entirely.</param>
		/// <returns></returns>
		public static string Left(this String source, int chars)
		{
			return source.Length < chars ? source : source.Substring(0, chars);
		}

		/// <summary>
		/// Takes the right x chars of the string.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="chars">Number of chars to take. A string containing less will return entirely.</param>
		/// <returns></returns>
		public static string Right(this String source, int chars)
		{
			return source.Length < chars ? source : source.Substring(source.Length - chars);
		}


		/// <summary>
		/// Returns the <paramref name="defaultValue"/> if <paramref name="source"/> is empty, or whitespace.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="defaultValue">The default value to return as an alternative.</param>
		/// <returns></returns>
		public static string IfWhiteSpace(this String source, string defaultValue)
		{
			return string.IsNullOrWhiteSpace(source) ? defaultValue : source;
		}


		/// <summary>
		/// Returns the portion of a string before the first <paramref name="stopBefore"/> character.
		/// Result does not include the search char. No character found returns the original.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="stopBefore">The character to stop before.</param>
		/// <returns></returns>
		public static string SubStringUntil(this String source, char stopBefore)
		{
			var stopLoc = source.IndexOf(stopBefore);
			return stopLoc > -1 ? source.Substring(0, stopLoc) : source;
		}
		
		/// <summary>
		/// Returns the portion of a string after the last <paramref name="startAfter"/> character.
		/// Result does not include the search char. No character found returns empty string.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="startAfter">The character to start after.</param>
		/// <returns></returns>
		public static string SubStringAfter(this String source, char startAfter)
		{
			var startLoc = source.LastIndexOf(startAfter);
			return startLoc > -1 && source.Length > startLoc ? source.Substring(startLoc + 1) : "";
		}

		/// <summary>
		/// Returns the portion of a string that sits between the first and last occurrence of the specified <paramref name="char"/>.
		/// Result does not include the search chars. If both boundaries aren't found, returns empty string.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="char"></param>
		/// <returns></returns>
		public static string SubStringBetween(this String source, char @char)
		{
			return source.SubStringBetween(@char, @char);
		}

		/// <summary>
		/// Returns the portion of a string that sits after the first <paramref name="startAfter"/> character and before the last <paramref name="stopBefore"/> character.
		/// Result does not include the search chars.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="startAfter">Portion is taken from after the first one of these.</param>
		/// <param name="stopBefore">Portion is taken from before the last one of these.</param>
		/// <returns></returns>
		public static string SubStringBetween(this String source, char startAfter, char stopBefore)
		{
			var startLoc = source.IndexOf(startAfter);
			var stopLoc = source.LastIndexOf(stopBefore);
			if (startLoc == -1 || stopLoc == -1) return "";
			return source.Substring(startLoc + 1, stopLoc - startLoc - 1);
		}

		/// <summary>
		/// Trims the boundaryString from the beginning and end of the source string if found on both ends.
		/// Useful for trimming wrapping quotes off a string, such as those in Excel generated TSV files.
		/// </summary>
		/// <param name="source">The input source string.</param>
		/// <param name="boundaryString">The string you want trimmed.</param>
		/// <returns></returns>
		public static string TrimBoundaries(this String source, string boundaryString)
		{
			var length = boundaryString.Length;
			if (source.StartsWith(boundaryString) && source.EndsWith(boundaryString))
			{
				source = source.Substring(length, source.Length - length * 2);
			}
			return source;
		}

		/// <summary>
		///     Returns a string array that contains the substrings in this string that are
		///     delimited by the specified Unicode character. A parameter
		///     specifies whether to return empty array elements.
		/// </summary>
		/// <param name="source">The source string.</param>
		/// <param name="separator">A Unicode character that delimits the substrings in this string, or null.</param>
		/// <param name="options">System.StringSplitOptions.RemoveEmptyEntries to omit empty array elements
		///     from the array returned; or System.StringSplitOptions.None to include empty
		///     array elements in the array returned.</param>
		/// <returns>An array whose elements contain the substrings in this string that are delimited by the separator.</returns>
		/// <exception cref="System.ArgumentException">options is not one of the System.StringSplitOptions values.</exception>
		public static string[] Split(this string source, char separator, StringSplitOptions options)
		{
			return source.Split(new[] {separator}, options);
		}

		/// <summary>
		///     Returns a string array that contains the substrings in this string that are
		///     delimited by the specified string. A parameter
		///     specifies whether to return empty array elements.
		/// </summary>
		/// <param name="source">The source string.</param>
		/// <param name="separator">A string that delimits the substrings in this string, or null.</param>
		/// <param name="options">System.StringSplitOptions.RemoveEmptyEntries to omit empty array elements
		///     from the array returned; or System.StringSplitOptions.None to include empty
		///     array elements in the array returned.</param>
		/// <returns>An array whose elements contain the substrings in this string that are delimited by the separator.</returns>
		/// <exception cref="System.ArgumentException">options is not one of the System.StringSplitOptions values.</exception>
		public static string[] Split(this string source, string separator, StringSplitOptions options)
		{
			return source.Split(new[] { separator }, options);
		}
	}
}
