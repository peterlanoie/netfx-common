using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Helpers
{
	public static class GuidHelper
	{

		/// <summary>
		/// Performs a very rudimentary test of a string as a Guid.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public static bool TryParse(string input, out Guid result)
		{
			char[] guidChars = "0123456789ABCDEFabcdef{}-".ToCharArray();
			int charCount = 0;
			result = Guid.Empty;
			foreach(char inChar in input)
			{
				switch(inChar)
				{
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
					case 'A':
					case 'B':
					case 'C':
					case 'D':
					case 'E':
					case 'F':
					case 'a':
					case 'b':
					case 'c':
					case 'd':
					case 'e':
					case 'f':
						// characters that make up the Guid
						charCount++;
						break;

					case ',':
					case 'x':
					case '{':
					case '}':
					case '(':
					case ')':
					case '-':
						// characters allowed in a Guid string, but aren't actually part of the Guid
						break;
					default:
						// any other characters
						return false;
				}
			}
			if(charCount >= 32)
			{
				try
				{
					result = new Guid(input);
					return true;
				}
				catch { }
			}
			return false;
		}

		/// <summary>
		/// Returns an array of Guids from a string list separated by the specified char(s).
		/// </summary>
		/// <param name="input"></param>
		/// <param name="separators"></param>
		/// <returns></returns>
		public static Guid[] GetGuidsFromList(string input, params char[] separators)
		{
			return GetGuidsFromList(input.Split(separators, StringSplitOptions.RemoveEmptyEntries));
		}

		/// <summary>
		/// Returns an array of Guids from a string list separated by the specified string(s).
		/// </summary>
		/// <param name="input"></param>
		/// <param name="separators"></param>
		/// <returns></returns>
		public static Guid[] GetGuidsFromList(string input, params string[] separators)
		{
			return GetGuidsFromList(input.Split(separators, StringSplitOptions.RemoveEmptyEntries));
		}

		public static Guid[] GetGuidsFromList(params string[] guids)
		{
			var guidList = new List<Guid>();
			foreach(var guidString in guids)
			{
				Guid guid;
				if(Guid.TryParse(guidString, out guid))
				{
					guidList.Add(guid);
				}
			}
			return guidList.ToArray();
		}
	}
}
