// **************************************************************************
// * $Id: IndexerResult.cs 39 2006-12-17 10:24:58Z bhuijben $
// * $HeadURL: http://sourceserversharp.googlecode.com/svn/trunk/src/Libraries/QQn.SourceServerSharp/Framework/IndexerResult.cs $
// **************************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace QQn.SourceServerSharp
{
	/// <summary>
	/// 
	/// </summary>
	public class IndexerResult
	{
		readonly int _indexedSymbolFiles;
		readonly int _indexedSourceFiles;
		readonly int _providersUsed;
		readonly bool _successFull;
		readonly string _errorMessage;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="success"></param>
		/// <param name="indexedSymbolFiles"></param>
		/// <param name="indexedSourceFiles"></param>
		/// <param name="providersUsed"></param>
		internal IndexerResult(bool success, int indexedSymbolFiles, int indexedSourceFiles, int providersUsed)
		{
			_successFull = success;
			_indexedSymbolFiles = indexedSymbolFiles;
			_indexedSourceFiles = indexedSourceFiles;
			_providersUsed = providersUsed;
		}

		internal IndexerResult(bool success, string errorMessage)
		{
			_successFull = success;
			_errorMessage = errorMessage;
		}

		/// <summary>
		/// 
		/// </summary>
		public bool Success
		{
			get { return _successFull; }
		}

		/// <summary>
		/// 
		/// </summary>
		public int IndexedSymbolFiles
		{
			get { return _indexedSymbolFiles; }
		}

		/// <summary>
		/// 
		/// </summary>
		public int IndexedSourceFiles
		{
			get { return _indexedSourceFiles; }
		}

		/// <summary>
		/// 
		/// </summary>
		public int ProvidersUsed
		{
			get { return _providersUsed; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string ErrorMessage
		{
			get { return _errorMessage; }
		}
	}
}
