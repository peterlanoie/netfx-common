using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Common.Caching
{

	public class InProcStaticFileWatchCacheProvider : InProcStaticCacheProvider
	{
		private string _beaconFileDir;

		/// <summary>
		/// Creates a new instance of this provider with the specified beacon file directory.
		/// </summary>
		/// <param name="beaconFileDir">The directory where beacon files to monitor for expiration will be created.</param>
		public InProcStaticFileWatchCacheProvider(string beaconFileDir)
		{
			_beaconFileDir = beaconFileDir;
		}

		public override object Store(string key, object item)
		{
			var filename = MakeWatchFileName(key);
			var filepath = Path.Combine(_beaconFileDir, filename);

			if (!File.Exists(filepath))
			{
				if (Directory.Exists(_beaconFileDir))
				{
					
				}
			}

			return base.Store(key, item);
		}

		public static string MakeWatchFileName(string key)
		{
			return string.Format("cacheBeacon-{0}", key);
		}
	}
}
