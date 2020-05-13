using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.DifferenceEngine
{
	public enum DiffSectionType
	{
		Copy,
		Insert,
		Delete
	}

	public struct DiffSection
	{
		private readonly DiffSectionType _Type;
		private readonly int _Length;

		public DiffSection(DiffSectionType type, int length)
		{
			_Type = type;
			_Length = length;
		}

		public DiffSectionType Type
		{
			get
			{
				return _Type;
			}
		}

		public int Length
		{
			get
			{
				return _Length;
			}
		}

		public override string ToString()
		{
			return string.Format("{0} {1}", Type, Length);
		}
	}
}
