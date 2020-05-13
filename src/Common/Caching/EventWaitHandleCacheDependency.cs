using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Common.Caching
{
	/// <summary>
	/// Defines a cache dependency mechanism using an event wait handle that can be used
	/// for interprocess signaling to expire cache values between applications.
	/// </summary>
	public class EventWaitHandleCacheDependency : CacheDependency
	{
		private readonly Thread _waitThread;
		private readonly EventWaitHandle _waitHandle;
		private bool _isRunning = false;
		private readonly ManualResetEvent _readyEvent;

		public override void Stop()
		{
			_isRunning = false;
			if (_waitThread == null) return;
			// if this is called from the wait thread, we shouldn't do anything
			if (_waitThread == Thread.CurrentThread) return;
			// in the event this is called from a different thread,
			// we should signal to stop monitoring
			if (_waitHandle != null) _waitHandle.Set();
			// and wait for the monitor thread to finish clean
			_waitThread.Join();
		}

		public EventWaitHandleCacheDependency(string key)
		{
			Key = key;
			_waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset, Key);
			_readyEvent = new ManualResetEvent(false);
			_isRunning = true;
			_waitThread = new Thread(WaitForExpireSignal);
			_waitThread.Start();
		}

		~EventWaitHandleCacheDependency()
		{
			_waitHandle.Dispose();
		}

		protected virtual void WaitForExpireSignal()
		{
			try
			{
				_readyEvent.Set();
				_waitHandle.WaitOne();
				if(_isRunning)
				{
					ExpireItem();
				}
				_waitHandle.Reset();
			}
			catch(ThreadAbortException)
			{
				// This is likely to happen when apps are shut down or recycled.
				_isRunning = false;
			}
		}

		public static void SignalExpiration(string key)
		{
			var waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset, key);
			waitHandle.Set();
			waitHandle.Dispose();
		}

		public virtual void WaitForReady()
		{
			_readyEvent.WaitOne();
		}
	}
}
