using System;
using Common.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Tests.ComponentModel.DataAnnotations
{
	[TestClass]
	public class EnumConverterTests
	{
		private EnumConverter<GoodEnum> _goodConverter;

		[TestInitialize]
		public void Setup()
		{
			_goodConverter = EnumConverter<GoodEnum>.GetConverter();
		}

		[TestMethod]
		public void FromStorageValueSingleMap()
		{
			Assert.AreEqual(GoodEnum.One, _goodConverter.FromStorageValue("uno"));
		}

		[TestMethod]
		public void FromStorageValueMultiMap()
		{
			Assert.AreEqual(GoodEnum.Two, _goodConverter.FromStorageValue("dos"));
		}

		[TestMethod]
		public void ToStorageValueSingleMap()
		{
			Assert.AreEqual("uno", _goodConverter.ToStorageValue(GoodEnum.One));
		}

		[TestMethod]
		public void ToStorageValueMultiMapWithPreferred()
		{
			Assert.AreEqual("dos", _goodConverter.ToStorageValue(GoodEnum.Two));
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void MultiMapWithoutPreferred()
		{
			EnumConverter<BadEnum>.GetConverter();
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void MultiMapMultiPreferred()
		{
			EnumConverter<BadEnum>.GetConverter();
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void EnumNoMappings()
		{
			EnumConverter<BlankEnum>.GetConverter();
		}

		private enum BlankEnum
		{
			One,

			Two,
		}

		private enum GoodEnum
		{
			[StringStorageValue("uno")]
			One,

			[StringStorageValue("dos", true)]
			[StringStorageValue("2")]
			Two,
		}

		private enum BadEnum
		{
			[StringStorageValue("tres")]
			[StringStorageValue("3")]
			Three,

			[StringStorageValue("quatro", true)]
			[StringStorageValue("4", true)]
			Four,
		}

	}
}
