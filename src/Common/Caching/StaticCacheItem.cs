using System;

namespace Common.Caching
{
	/// <summary>
	/// Defines a generic for an item that is cached as a static instance.
	/// This type provides the thread locking/blocking to marshal requests within a single app domain.
	/// Consumer is responsible for storing an instance of this type to a static member.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class StaticCacheItem<T>
	{
		private T Data { get; set; }
		private readonly object _lock = new object();

		/// <summary>
		/// Gets the catched data if available, otherwise calls <paramref name="createAction"/> delegate to create it, then returns it.
		/// </summary>
		/// <param name="createAction"></param>
		/// <returns></returns>
		public T GetData(Func<T> createAction)
		{
			if(Data == null)
			{
				lock(_lock)
				{
					// second null check to eliminate additional threads which may also be waiting to create this
					if(Data == null)
					{
						// return the created result before lock release to avoid conflict with nullification in flush method
						Data = createAction();
					}
				}
			}
			return Data;
		}
	}
}