using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common.Caching;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.Tests.Caching
{
	[TestClass]
	public class EventWaitHandleDependencyTests
	{
		[TestMethod]
		public void ExpireEventFiresForMatchedKey()
		{
			const string key = "expectedKey";
			var dep = new EventWaitHandleCacheDependency(key);
			var expiredKey = "";
			dep.OnExpireItem += (x, y) => expiredKey = y.Key;

			dep.WaitForReady();
			EventWaitHandleCacheDependency.SignalExpiration(key);
			Thread.Sleep(100);
			Assert.AreEqual(key, expiredKey);
		}

		[TestMethod]
		public void ExpireEventDoesntFireForDifferentKey()
		{
			const string key = "expectedKey";
			var dep = new EventWaitHandleCacheDependency(key);
			var expireFired = false;
			dep.OnExpireItem += (x, y) => expireFired = true;
			EventWaitHandleCacheDependency.SignalExpiration("differentKey");

			Thread.Sleep(100);
			Assert.IsFalse(expireFired);
		}

	}
}
