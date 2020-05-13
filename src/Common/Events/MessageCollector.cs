using System.Collections.Generic;
using System.Linq;

namespace Common.Events
{
	public class MessageCollector
	{
		public TypedMessageList AllMessages { get; set; }

		public IEnumerable<string> ErrorMessages { get { return GetFilteredMessages(MessageLevelType.Error); } }
		public IEnumerable<string> WarningMessages { get { return GetFilteredMessages(MessageLevelType.Warning); } }
		public IEnumerable<string> InfoMessages { get { return GetFilteredMessages(MessageLevelType.Info); } }
		public IEnumerable<string> DebugMessages { get { return GetFilteredMessages(MessageLevelType.Debug); } }
		public IEnumerable<string> TraceMessages { get { return GetFilteredMessages(MessageLevelType.Trace); } }

		public MessageCollector(Messenger messenger)
		{
			AllMessages = new TypedMessageList();
			messenger.OnDebug += AddDebug;
			messenger.OnError += AddError;
			messenger.OnInfo += AddInfo;
			messenger.OnTrace += AddTrace;
			messenger.OnWarning += AddWarning;
		}

		public void AddError(string format, params object[] args)
		{
			AllMessages.AddError(string.Format(format, args));
		}

		public void AddWarning(string format, params object[] args)
		{
			AllMessages.AddWarning(string.Format(format, args));
		}

		public void AddInfo(string format, params object[] args)
		{
			AllMessages.AddInfo(string.Format(format, args));
		}

		public void AddDebug(string format, params object[] args)
		{
			AllMessages.AddDebug(string.Format(format, args));
		}

		public void AddTrace(string format, params object[] args)
		{
			AllMessages.AddTrace(string.Format(format, args));
		}

		private void AddError(object sender, string messagetext)
		{
			AllMessages.AddError(messagetext);
		}

		private void AddWarning(object sender, string messagetext)
		{
			AllMessages.AddWarning(messagetext);
		}

		private void AddInfo(object sender, string messagetext)
		{
			AllMessages.AddInfo(messagetext);
		}

		private void AddDebug(object sender, string messagetext)
		{
			AllMessages.AddDebug(messagetext);
		}

		private void AddTrace(object sender, string messagetext)
		{
			AllMessages.AddTrace(messagetext);
		}

		private IEnumerable<string> GetFilteredMessages(MessageLevelType type)
		{
			return AllMessages.Where(x => x.Level == type).Select(x => x.Text);
		}
	}
}