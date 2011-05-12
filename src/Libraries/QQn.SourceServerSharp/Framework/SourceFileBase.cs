// **************************************************************************
// * $Id: SourceFileBase.cs 39 2006-12-17 10:24:58Z bhuijben $
// * $HeadURL: http://sourceserversharp.googlecode.com/svn/trunk/src/Libraries/QQn.SourceServerSharp/Framework/SourceFileBase.cs $
// **************************************************************************

namespace QQn.SourceServerSharp.Framework
{
	using System;
	using System.IO;

	public abstract class SourceFileBase : IComparable
	{
		protected SourceFileBase(string filename)
		{
			if (string.IsNullOrEmpty(filename))
				throw new ArgumentNullException("filename");

			this.File = new FileInfo(filename);
		}

		public FileInfo File { get; private set; }
		public string FullName
		{
			get { return this.File.FullName; }
		}
		public bool Exists
		{
			get { return this.File.Exists; }
		}
		public override string ToString()
		{
			return this.FullName;
		}

		public int CompareTo(SourceFileBase other)
		{
			return string.Compare(this.FullName, other.FullName, StringComparison.InvariantCultureIgnoreCase);
		}

		public bool Equals(SourceFileBase other)
		{
			return other != null && string.Equals(this.FullName, other.FullName, StringComparison.InvariantCultureIgnoreCase);
		}

		public int CompareTo(object obj)
		{
			return this.CompareTo(obj as SourceFileBase);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as SourceFileBase);
		}

		public override int GetHashCode()
		{
			return StringComparer.InvariantCultureIgnoreCase.GetHashCode(this.FullName);
		}
	}
}