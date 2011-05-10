// **************************************************************************
// * $Id: SourceIndexToolException.cs 39 2006-12-17 10:24:58Z bhuijben $
// * $HeadURL: http://sourceserversharp.googlecode.com/svn/trunk/src/Libraries/QQn.SourceServerSharp/Framework/SourceIndexToolException.cs $
// **************************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace QQn.SourceServerSharp.Framework
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class SourceIndexToolException : SourceIndexException
	{
		/// <summary>
		/// 
		/// </summary>
		public SourceIndexToolException()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="toolname"></param>
		/// <param name="message"></param>
		public SourceIndexToolException(string toolname, string message)
			: base(string.Format("{0} failed: {1}", toolname, message))
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="toolname"></param>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public SourceIndexToolException(string toolname, string message, Exception inner)
			: base(string.Format("{0} failed: {1}", toolname, message), inner)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected SourceIndexToolException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
