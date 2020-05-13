namespace Common.Caching
{
	public delegate void CacheItemExpirationEventHandler(object sender, CacheItemExpirationEventHandlerArgs args);

	public class CacheItemExpirationEventHandlerArgs
	{
		public CacheItemExpirationEventHandlerArgs(string key)
		{
			Key = key;
		}

		public string Key { get; set; }
	}
}