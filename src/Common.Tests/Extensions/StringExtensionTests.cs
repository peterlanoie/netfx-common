using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Tests.Extensions
{
	[TestClass]
	public class StringExtensionTests
	{
		[TestMethod]
		public void LeftSegmentReturn()
		{
			var source = "This is the source string.";
			var result = source.Left(7);
			Assert.AreEqual("This is", result);
		}

		[TestMethod]
		public void RightSegmentReturn()
		{
			var source = "This is the source string.";
			var result = source.Right(7);
			Assert.AreEqual("string.", result);
		}

		[TestMethod]
		public void LeftReturnWhole()
		{
			var source = "short string";
			var result = source.Left(20);
			Assert.AreEqual(source, result);
		}

		[TestMethod]
		public void RightReturnWhole()
		{
			var source = "short string";
			var result = source.Right(20);
			Assert.AreEqual(source, result);
		}

		private const string _fragmentSource = ":first;second;third;fourth:";

		[TestMethod]
		public void SubStringUntil()
		{
			Assert.AreEqual(":first", _fragmentSource.SubStringUntil(';'));
		}

		[TestMethod]
		public void SubStringUntilFirstChar()
		{
			Assert.AreEqual("", _fragmentSource.SubStringUntil(':'));
		}

		[TestMethod]
		public void SubStringUntilNotFound()
		{
			Assert.AreEqual(_fragmentSource, _fragmentSource.SubStringUntil('~'));
		}

		[TestMethod]
		public void SubStringAfter()
		{
			Assert.AreEqual("fourth:", _fragmentSource.SubStringAfter(';'));
		}

		[TestMethod]
		public void SubStringAfterEmpty()
		{
			Assert.AreEqual("", _fragmentSource.SubStringAfter(':'));
		}

		[TestMethod]
		public void SubStringAfterNotFound()
		{
			Assert.AreEqual("", _fragmentSource.SubStringAfter('~'));
		}

		[TestMethod]
		public void SubStringBetween1()
		{
			Assert.AreEqual("first;second;third;fourth", _fragmentSource.SubStringBetween(':', ':'));
		}

		[TestMethod]
		public void SubStringBetween2()
		{
			Assert.AreEqual("second;third", _fragmentSource.SubStringBetween(';', ';'));
		}

		[TestMethod]
		public void SubStringBetweenNotFound1()
		{
			Assert.AreEqual("", _fragmentSource.SubStringBetween('~', ':'));
		}

		[TestMethod]
		public void SubStringBetweenNotFound2()
		{
			Assert.AreEqual("", _fragmentSource.SubStringBetween(':', '~'));
		}

		[TestMethod]
		public void SubStringBetweenSameChar1()
		{
			Assert.AreEqual("first;second;third;fourth", _fragmentSource.SubStringBetween(':'));
		}

		[TestMethod]
		public void SubStringBetweenSameChar2()
		{
			Assert.AreEqual("second;third", _fragmentSource.SubStringBetween(';'));
		}

		[TestMethod]
		public void SubStringBetweenSameCharNotFound()
		{
			Assert.AreEqual("", _fragmentSource.SubStringBetween('~'));
		}

		[TestMethod]
		public void TrimBoundariesExistsSingleChar()
		{
			Assert.AreEqual("boundary test", ":boundary test:".TrimBoundaries(":"));
		}

		[TestMethod]
		public void TrimBoundariesExistsMultipleChar()
		{
			Assert.AreEqual("boundary test", ":::boundary test:::".TrimBoundaries(":::"));
		}

		[TestMethod]
		public void TrimBoundariesExistsMultipleChar2()
		{
			Assert.AreEqual("::boundary test::", ":::boundary test:::".TrimBoundaries(":"));
		}

		[TestMethod]
		public void TrimBoundariesExistsLeaveOrphanLeft()
		{
			Assert.AreEqual(":boundary test", ":boundary test".TrimBoundaries(":"));
		}

		[TestMethod]
		public void TrimBoundariesExistsLeaveOrphanRight()
		{
			Assert.AreEqual("boundary test:", "boundary test:".TrimBoundaries(":"));
		}

	}
}
