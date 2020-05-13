using System;
using System.Collections.Generic;

namespace Common.Events
{
	[Serializable]
	public class TypedMessage
	{
		public MessageLevelType Level { get; set; }
		public string Text { get; set; }
		public DateTime CreateTime { get; set; }

		public TypedMessage(MessageLevelType level, string text)
		{
			Level = level;
			Text = text;
			CreateTime = new DateTime();
		}

		public static TypedMessage Error(string text)
		{
			return new TypedMessage(MessageLevelType.Error, text);
		}

		public static TypedMessage Warning(string text)
		{
			return new TypedMessage(MessageLevelType.Warning, text);
		}

		public static TypedMessage Info(string text)
		{
			return new TypedMessage(MessageLevelType.Info, text);
		}

		public static TypedMessage Debug(string text)
		{
			return new TypedMessage(MessageLevelType.Debug, text);
		}

		public static TypedMessage Trace(string text)
		{
			return new TypedMessage(MessageLevelType.Trace, text);
		}

	}

	public class TypedMessageList : List<TypedMessage>
	{
		public string AddError(string text)
		{
			return Add(MessageLevelType.Error, text);
		}

		public string AddWarning(string text)
		{
			return Add(MessageLevelType.Warning, text);
		}

		public string AddInfo(string text)
		{
			return Add(MessageLevelType.Info, text);
		}

		public string AddDebug(string text)
		{
			return Add(MessageLevelType.Debug, text);
		}

		public string AddTrace(string text)
		{
			return Add(MessageLevelType.Trace, text);
		}

		public string Add(MessageLevelType level, string text)
		{
			var message = new TypedMessage(level, text);
			Add(message);
			return text;
		}
	}

	[Serializable]
	public enum MessageLevelType
	{
		Error = 5,
		Warning = 4,
		Info = 3,
		Debug = 2,
		Trace = 1,
	}
}