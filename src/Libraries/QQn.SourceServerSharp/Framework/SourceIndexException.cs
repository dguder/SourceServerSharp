// **************************************************************************
// * $Id: SourceIndexException.cs 39 2006-12-17 10:24:58Z bhuijben $
// * $HeadURL: http://sourceserversharp.googlecode.com/svn/trunk/src/Libraries/QQn.SourceServerSharp/Framework/SourceIndexException.cs $
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
	public class SourceIndexException : ApplicationException
	{
		/// <summary>
		/// 
		/// </summary>
		public SourceIndexException()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public SourceIndexException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public SourceIndexException(string message, Exception inner)
			: base(message, inner)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected SourceIndexException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
