// **************************************************************************
// * $Id: SymbolFile.cs 39 2006-12-17 10:24:58Z bhuijben $
// * $HeadURL: http://sourceserversharp.googlecode.com/svn/trunk/src/Libraries/QQn.SourceServerSharp/Framework/SymbolFile.cs $
// **************************************************************************

using System;
using System.Collections.Generic;
using System.Configuration;

namespace QQn.SourceServerSharp.Framework
{
	/// <summary>
	/// 
	/// </summary>
	public class SymbolFile : SourceFileBase, IComparable<SourceFile>, IEquatable<SourceFile>
	{
		readonly List<SourceFile> _sourceFiles = new List<SourceFile>();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filename"></param>
		public SymbolFile(string filename)
			: base(filename)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		public IList<SourceFile> SourceFiles
		{
			get { return _sourceFiles; }
		}

		internal void AddSourceFile(SourceFile sourceFile)
		{
			if(sourceFile == null)
				throw new ArgumentNullException("sourceFile");

			if (!_sourceFiles.Contains(sourceFile))
				_sourceFiles.Add(sourceFile);
		}
			
		#region ## IComparable<SourceFile>, IEquatable<SourceFile>

		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int CompareTo(SourceFile other)
		{
			return base.CompareTo(other);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(SourceFile other)
		{
			return base.Equals(other);
		}
		#endregion ## IComparable<SourceFile>, IEquatable<SourceFile>
	}
}
