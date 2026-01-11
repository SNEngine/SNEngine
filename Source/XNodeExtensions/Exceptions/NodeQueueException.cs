using System;

namespace SiphoinUnityHelpers.XNodeExtensions.Exceptions
{

    [Serializable]
	public class NodeQueueException : Exception
	{
		public NodeQueueException() { }
		public NodeQueueException(string message) : base(message) { }
		public NodeQueueException(string message, Exception inner) : base(message, inner) { }
		protected NodeQueueException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
