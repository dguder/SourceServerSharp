// **************************************************************************
// * $Id: SourceFileBase.cs 39 2006-12-17 10:24:58Z bhuijben $
// * $HeadURL: http://sourceserversharp.googlecode.com/svn/trunk/src/Libraries/QQn.SourceServerSharp/Framework/SourceFileBase.cs $
// **************************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace QQn.SourceServerSharp.Framework
{
	/// <summary>
	/// Baseclass for <see cref="SourceFile"/> and <see cref="SymbolFile"/>
	/// </summary>
	public abstract class SourceFileBase : IComparable
	{
		readonly FileInfo _fileInfo;

		/// <summary>
		/// Creates a new SourceFile object for the specified file
		/// </summary>
		/// <param name="filename"></param>
		protected SourceFileBase(string filename)
		{
			if (string.IsNullOrEmpty(filename))
				throw new ArgumentNullException("filename");

			_fileInfo = new FileInfo(filename);
		}

		/// <summary>
		/// 
		/// </summary>
		public FileInfo File
		{
			get { return _fileInfo; }
		}

		/// <summary>
		/// Gets the fullname of the file
		/// </summary>
		public string FullName
		{
			get { return File.FullName; }
		}

		/// <summary>
		/// Gets a (cached) boolean indicating whether the file exists on disk
		/// </summary>
		public bool Exists
		{
			get { return File.Exists; }
		}

		/// <summary>
		/// returns <see cref="FullName"/>
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return FullName;
		}

		#region ## Compare members (specialized by base classes; for generics)

		/// <summary>
		/// Compares two <see cref="SourceFile"/> by its <see cref="FullName"/>
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int CompareTo(SourceFileBase other)
		{
			return string.Compare(FullName, other.FullName, StringComparison.InvariantCultureIgnoreCase);
		}

		/// <summary>
		/// Compares two <see cref="SourceFile"/> by its <see cref="FullName"/>
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(SourceFileBase other)
		{
			if (other == null)
				return false;

			return string.Equals(FullName, other.FullName, StringComparison.InvariantCultureIgnoreCase);
		}

		#endregion ## Compare members (specialized by base classes; for generics)

		#region ## .Net 1.X compatible compare Members

		/// <summary>
		/// Compares two <see cref="SourceFile"/> by its <see cref="FullName"/>
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			// Use typed version
			return CompareTo(obj as SourceFileBase);
		}

		/// <summary>
		/// Compares two <see cref="SourceFile"/> by its <see cref="FullName"/>
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			return Equals(obj as SourceFileBase);
		}
		#endregion ## .Net 1.X compatible compare Members

		/// <summary>
		/// Gets the hashcode of the <see cref="SourceFile"/>
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return StringComparer.InvariantCultureIgnoreCase.GetHashCode(FullName);
		}
	}
}
