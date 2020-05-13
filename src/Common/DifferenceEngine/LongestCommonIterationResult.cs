namespace Common.DifferenceEngine
{
	/// <summary>
	/// Because this is a struct, there should be an implicit default constructor if needed.
	/// This just holds the information about an iteration match in the diff engine.
	/// </summary>
	public struct LongestCommonIterationResult
	{
		private readonly bool _success;
		private readonly int _positionInFirstCollection;
		private readonly int _positionInSecondCollection;
		private readonly int _length;

		public LongestCommonIterationResult(
			int positionInFirstCollection,
			int positionInSecondCollection,
			int length)
		{
			_success = true;
			_positionInFirstCollection = positionInFirstCollection;
			_positionInSecondCollection = positionInSecondCollection;
			_length = length;
		}

		public bool Success
		{
			get
			{
				return _success;
			}
		}

		public int PositionInFirstCollection
		{
			get
			{
				return _positionInFirstCollection;
			}
		}

		public int PositionInSecondCollection
		{
			get
			{
				return _positionInSecondCollection;
			}
		}

		public int Length
		{
			get
			{
				return _length;
			}
		}

		public override string ToString()
		{
			if (Success)
				return string.Format(
					"LCS ({0}, {1} x{2})",
					PositionInFirstCollection,
					PositionInSecondCollection,
					Length);
			else
				return "LCS (-)";
		}
	}
}
