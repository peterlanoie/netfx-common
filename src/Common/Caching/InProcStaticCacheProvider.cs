using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Caching
{
	/// <summary>
	/// Defines a cache provider that stores objects in in-process memory.
	/// This provider has no automatic expiration.
	/// </summary>
	public class InProcStaticCacheProvider : CacheProvider
	{
		private static Hashtable _cache;
		private static readonly object CacheCreateLock = new object();

		private static Hashtable Cache
		{
			get
			{
				if(_cache == null)
				{
					lock(CacheCreateLock)
					{
						if(_cache == null)
						{
							// second check - another process may have set this already while we were blocked.
							// ReSharper disable PossibleMultipleWriteAccessInDoubleCheckLocking
							_cache = new Hashtable();
							// ReSharper restore PossibleMultipleWriteAccessInDoubleCheckLocking
						}
					}
				}
				return _cache;
			}
		}

		public override object Get(string key)
		{
			return Cache[key];
		}

		public object Get(string key, Func<object> createCallback)
		{
			return Get(key) ?? Store(key, createCallback());
		}

		public override object Store(string key, object item)
		{
			lock(Cache.SyncRoot)
			{
				Cache[key] = item;
			}
			return item;
		}

		protected override void DeleteItem(string key)
		{
			lock(Cache.SyncRoot)
			{
				Cache.Remove(key);
			}
			//if(ItemExpired != null)
			//{
			//    ItemExpired(this, new CacheItemExpiredEventHandlerArgs(key));
			//}
		}

		public override bool HasKey(string key)
		{
			return _cache.ContainsKey(key);
		}

		//public event CacheItemExpiredEventHandler ItemExpired;
	}
}
