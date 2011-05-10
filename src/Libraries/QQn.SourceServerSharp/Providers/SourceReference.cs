// **************************************************************************
// * $Id: SourceReference.cs 39 2006-12-17 10:24:58Z bhuijben $
// * $HeadURL: http://sourceserversharp.googlecode.com/svn/trunk/src/Libraries/QQn.SourceServerSharp/Providers/SourceReference.cs $
// **************************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using QQn.SourceServerSharp.Framework;

namespace QQn.SourceServerSharp.Providers
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class SourceReference
	{
		readonly SourceProvider _provider;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="provider"></param>
		protected SourceReference(SourceProvider provider)
		{
			if (provider == null)
				throw new ArgumentNullException("provider");

			_provider = provider;
		}

		/// <summary>
		/// 
		/// </summary>
		public SourceProvider SourceProvider
		{
			get { return _provider; }
		}
		
		/// <summary>
		/// Gets a list of string entries for the specified sourcefile which are used for 
		/// %VAR3%..$VARx% in the extract script
		/// </summary>
		/// <returns></returns>
		/// <remarks>The number of entries returned MUST be less than or equal to the number returned 
		/// by <see cref="SourceProvider"/>.SourceEntryVariableCount</remarks>
		public abstract string[] GetSourceEntries();
	}
}
