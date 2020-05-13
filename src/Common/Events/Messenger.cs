using System;
using System.Linq;
using System.Text;

namespace Common.Events
{
	public delegate void MessengerEventDelegate(object sender, string messageText);

	/// <summary>
	/// Provides events for typical leveled messages.
	/// Helpful when you aren't using typical logging mechanisms.
	/// </summary>
	public class Messenger
	{
		/// <summary>
		/// Raised for an Error message.
		/// </summary>
		public event MessengerEventDelegate OnError;

		/// <summary>
		/// Raised for a warning message.
		/// </summary>
		public event MessengerEventDelegate OnWarning;

		/// <summary>
		/// Raised for an informational message.
		/// </summary>
		public event MessengerEventDelegate OnInfo;
		
		/// <summary>
		/// Raised for debug messages.
		/// </summary>
		public event MessengerEventDelegate OnDebug;
		
		/// <summary>
		/// Raised for trace messages.
		/// </summary>
		public event MessengerEventDelegate OnTrace;

		/// <summary>
		/// Signals an error message event.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public string Error(string format, params object[] args)
		{
			return RaiseMessageEvent(OnError, format, args);
		}

		/// <summary>
		/// Signals a warning message event.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public string Warning(string format, params object[] args)
		{
			return RaiseMessageEvent(OnWarning, format, args);
		}

		/// <summary>
		/// Signals an info message event.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public string Info(string format, params object[] args)
		{
			return RaiseMessageEvent(OnInfo, format, args);
		}

		/// <summary>
		/// Signals a debug message event.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public string Debug(string format, params object[] args)
		{
			return RaiseMessageEvent(OnDebug, format, args);
		}

		/// <summary>
		/// Signals a trace message event.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public string Trace(string format, params object[] args)
		{
			return RaiseMessageEvent(OnTrace, format, args);
		}

		private string RaiseMessageEvent(MessengerEventDelegate handler, string format, params object[] args)
		{
			var message = string.Format(format, args);
			if(handler != null)
			{
				handler(this, message);
			}
			return message;
		}

		/// <summary>
		/// Creates a new instance of a message collector that collects all messages generated on this messenger.
		/// </summary>
		/// <returns></returns>
		public MessageCollector NewCollector()
		{
			return new MessageCollector(this);;
		}
	}
}
