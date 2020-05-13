using System.Collections.Generic;

namespace Common.DifferenceEngine
{
	/// <summary>
	/// I found this at this site: http://devdirective.com/post/91/creating-a-reusable-though-simple-diff-implementation-in-csharp-part-1
	/// It is a 3 part (part 4 was never posted) series of articles on this subject written by user LasseVK.
	/// </summary>
	public class DiffEngine
	{
		/// <summary>
		/// This basically finds the longest common series of <T> objects in 2 Lists.
		/// It is basically a brute force search from a start to end index in each list to see how much of each list is considered
		/// equal.  If you need it, you can pass in a IEqualityComparer for your type to do the comparisons./>
		/// </summary>
		/// <typeparam name="T">Type in the lists you are searching.</typeparam>
		/// <param name="firstCollection"></param>
		/// <param name="firstStart"></param>
		/// <param name="firstEnd"></param>
		/// <param name="secondCollection"></param>
		/// <param name="secondStart"></param>
		/// <param name="secondEnd"></param>
		/// <param name="equalityComparer"></param>
		/// <returns></returns>
		private LongestCommonIterationResult FindLongestCommonIteration<T>(
		IList<T> firstCollection, int firstStart, int firstEnd,
		IList<T> secondCollection, int secondStart, int secondEnd,
		IEqualityComparer<T> equalityComparer)
		{
			// default result, if we can't find anything
			var result = new LongestCommonIterationResult();

			for (int index1 = firstStart; index1 < firstEnd; index1++)
			{
				for (int index2 = secondStart; index2 < secondEnd; index2++)
				{
					if (equalityComparer.Equals(
						firstCollection[index1],
						secondCollection[index2]))
					{
						int length = CountEqual(
							firstCollection, index1, firstEnd,
							secondCollection, index2, secondEnd,
							equalityComparer);

						// Is longer than what we already have --> record new LCS
						if (length > result.Length)
						{
							result = new LongestCommonIterationResult(
								index1,
								index2,
								length);
						}
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Find the number of items that are equal in two lists.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="firstCollection"></param>
		/// <param name="firstPosition"></param>
		/// <param name="firstEnd"></param>
		/// <param name="secondCollection"></param>
		/// <param name="secondPosition"></param>
		/// <param name="secondEnd"></param>
		/// <param name="equalityComparer"></param>
		/// <returns></returns>
		private int CountEqual<T>(
			IList<T> firstCollection, int firstPosition, int firstEnd,
			IList<T> secondCollection, int secondPosition, int secondEnd,
			IEqualityComparer<T> equalityComparer)
		{
			int length = 0;
			while (firstPosition < firstEnd
				&& secondPosition < secondEnd)
			{
				if (!equalityComparer.Equals(
					firstCollection[firstPosition],
					secondCollection[secondPosition]))
				{
					break;
				}

				firstPosition++;
				secondPosition++;
				length++;
			}
			return length;
		}

		/// <summary>
		/// Return the difference between 2 strings.
		/// </summary>
		/// <param name="firstString"></param>
		/// <param name="firstStart"></param>
		/// <param name="firstEnd"></param>
		/// <param name="secondString"></param>
		/// <param name="secondStart"></param>
		/// <param name="secondEnd"></param>
		/// <returns></returns>
		public IEnumerable<DiffSection> DiffStrings(
			string firstString, int firstStart, int firstEnd,
			string secondString, int secondStart, int secondEnd)
		{
			return Diff(
				firstString.ToCharArray(), firstStart, firstEnd,
				secondString.ToCharArray(), secondStart, secondEnd,
				EqualityComparer<char>.Default
				);
		}

		/// <summary>
		/// This returns, in order, the types of differences and the number of items for that type.
		/// You can iterate through the result set and come up with the steps for the differences between two lists.
		/// A type of Copy means that this part of the list was equal between the two lists and should be copied as is.
		/// A type of Insert means that list 2 contained a number of items that weren't in list 1.
		/// A type of Delete means that list 1 contained a number of items that weren't in list 2.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="firstCollection"></param>
		/// <param name="firstStart"></param>
		/// <param name="firstEnd"></param>
		/// <param name="secondCollection"></param>
		/// <param name="secondStart"></param>
		/// <param name="secondEnd"></param>
		/// <param name="equalityComparer"></param>
		/// <returns></returns>
		public IEnumerable<DiffSection> Diff<T>(
		IList<T> firstCollection, int firstStart, int firstEnd,
		IList<T> secondCollection, int secondStart, int secondEnd,
		IEqualityComparer<T> equalityComparer)
		{
			var lcs = FindLongestCommonIteration(
				firstCollection, firstStart, firstEnd,
				secondCollection, secondStart, secondEnd,
				equalityComparer);

			if (lcs.Success)
			{
				// deal with the section before
				var sectionsBefore = Diff(
					firstCollection, firstStart, lcs.PositionInFirstCollection,
					secondCollection, secondStart, lcs.PositionInSecondCollection,
					equalityComparer);
				foreach (var section in sectionsBefore)
					yield return section;

				// output the copy operation
				yield return new DiffSection(
					DiffSectionType.Copy,
					lcs.Length);

				// deal with the section after
				var sectionsAfter = Diff(
					firstCollection, lcs.PositionInFirstCollection + lcs.Length, firstEnd,
					secondCollection, lcs.PositionInSecondCollection + lcs.Length, secondEnd,
					equalityComparer);
				foreach (var section in sectionsAfter)
					yield return section;

				yield break;
			}

			// You only reach this after there are no matches in the lists.
			if (firstStart < firstEnd)
			{
				// we got content from first collection --> deleted
				yield return new DiffSection(
					DiffSectionType.Delete,
					firstEnd - firstStart);
			}
			if (secondStart < secondEnd)
			{
				// we got content from second collection --> inserted
				yield return new DiffSection(
					DiffSectionType.Insert,
					secondEnd - secondStart);
			}
		}
    }
}
