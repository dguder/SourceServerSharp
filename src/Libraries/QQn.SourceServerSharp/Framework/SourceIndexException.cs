namespace QQn.SourceServerSharp.Framework
{
	using System;
	using System.Runtime.Serialization;

	public class SourceIndexException : ApplicationException
	{
		public SourceIndexException()
		{
		}

		public SourceIndexException(string message)
			: base(message)
		{
		}

		public SourceIndexException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected SourceIndexException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}