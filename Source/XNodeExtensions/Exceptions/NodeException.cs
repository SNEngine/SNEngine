using SiphoinUnityHelpers.XNodeExtensions;
using System;

namespace SiphoinUnityHelpers.Exceptions
{

    [Serializable]
	public class NodeException : Exception
	{
		public BaseNode ThrowNode { get; private set; }
		public NodeException() { }
		public NodeException(string message) : base(message)
		{

		}

        public NodeException(string message, BaseNode ownerThrow) : base(message)
        {
			ThrowNode = ownerThrow;
        }
        public NodeException(string message, Exception inner) : base(message, inner) { }
		protected NodeException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
