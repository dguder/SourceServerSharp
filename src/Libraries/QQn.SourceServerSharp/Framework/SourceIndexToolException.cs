namespace QQn.SourceServerSharp.Framework
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class SourceIndexToolException : SourceIndexException
	{
		public SourceIndexToolException()
		{
		}

		public SourceIndexToolException(string toolname, string message)
			: base(string.Format("{0} failed: {1}", toolname, message))
		{
		}

		public SourceIndexToolException(string toolname, string message, Exception inner)
			: base(string.Format("{0} failed: {1}", toolname, message), inner)
		{
		}

		protected SourceIndexToolException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}