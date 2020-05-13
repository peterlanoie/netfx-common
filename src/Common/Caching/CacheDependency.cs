using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Caching
{
	public abstract class CacheDependency
	{
		internal string Key { get; set; }

		/// <summary>
		/// Event raised when a cached item expires or otherwise becomes invalidated.
		/// </summary>
		public event CacheItemExpirationEventHandler OnExpireItem;

		/// <summary>
		/// Stops doing whatever this is doing to determining cache dependency state.
		/// </summary>
		public abstract void Stop();

		protected void ExpireItem()
		{
			if(OnExpireItem != null)
			{
				OnExpireItem(this, new CacheItemExpirationEventHandlerArgs(Key));
			}
		}

	}
}
