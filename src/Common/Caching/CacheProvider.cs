using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Caching
{
	public abstract class CacheProvider
	{
		private List<CacheDependency> _dependencies = new List<CacheDependency>();

		/// <summary>
		/// Stores the <paramref name="item"/> using the <paramref name="key"/> identifier.
		/// </summary>
		/// <param name="key">The key that identifies the item in the concrete cache provider.</param>
		/// <param name="item">The item to cache.</param>
		/// <returns>The stored object. Useful for operation/assignment chaining.</returns>
		public abstract object Store(string key, object item);

		/// <summary>
		/// Stores the <paramref name="item"/> using the <paramref name="key"/> identifier with 
		/// dependencies for expiration strategies.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="item"></param>
		/// <param name="dependencies">Zero or more depencies used to expire this item being added.</param>
		/// <returns></returns>
		public object Store(string key, object item, params CacheDependency[] dependencies)
		{
			foreach(var dependency in dependencies)
			{
				dependency.Key = key;
				dependency.OnExpireItem += dependency_OnExpireItem;
				_dependencies.Add(dependency);
			}
			var defaultDependency = GetDefaultDependency(key);
			if(defaultDependency != null)
			{
				_dependencies.Add(defaultDependency);
				defaultDependency.OnExpireItem += dependency_OnExpireItem;
			}

			return Store(key, item);
		}

		void dependency_OnExpireItem(object sender, CacheItemExpirationEventHandlerArgs args)
		{
			// Use the inner delete here to avoid invocation of external actions on derived type
			InnerDelete(args.Key);
		}

		/// <summary>
		/// Gets an item from the cache, or null if missing.
		/// </summary>
		/// <param name="key">The item identifier.</param>
		/// <returns></returns>
		public abstract object Get(string key);

		/// <summary>
		/// Gets the item for the provided key. 
		/// If item doesn't exist, calls <paramref name="createCallback"/> to create it and adds the returned value to the cache.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="createCallback"></param>
		/// <param name="dependencies">Zero or more depencies used to expire this item being added.</param>
		/// <returns></returns>
		public object Get(string key, Func<object> createCallback, params CacheDependency[] dependencies)
		{
			return Get(key) ?? Store(key, createCallback(), dependencies);
		}

		/// <summary>
		/// When implemented by a derived class, deletes the item identified by the <paramref name="key"/> from the cache store.
		/// </summary>
		/// <param name="key"></param>
		protected abstract void DeleteItem(string key);

		/// <summary>
		/// Delete the item identified by the <paramref name="key"/> from the cache.
		/// </summary>
		/// <param name="key"></param>
		public virtual void Delete(string key)
		{
			InnerDelete(key);
		}

		private void InnerDelete(string key)
		{
			var dependencies = _dependencies.Where(x => x.Key == key).ToList();
			foreach(var dependency in dependencies)
			{
				dependency.Stop();
				_dependencies.Remove(dependency);
			}
			DeleteItem(key);
			if (OnItemDeleted != null)
			{
				OnItemDeleted(this, new CacheItemExpirationEventHandlerArgs(key));
			}
		}

		/// <summary>
		/// Gets whether or not the provided key is present in the cache.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public abstract bool HasKey(string key);

		/// <summary>
		/// Gets a default dependency to use for the provided key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public virtual CacheDependency GetDefaultDependency(string key)
		{
			return null;
		}

		/// <summary>
		/// Event raised when a cached item is deleted.
		/// </summary>
		public event CacheItemExpirationEventHandler OnItemDeleted;
	}

}
