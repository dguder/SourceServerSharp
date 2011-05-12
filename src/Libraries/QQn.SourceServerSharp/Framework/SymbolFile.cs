namespace QQn.SourceServerSharp.Framework
{
	using System;
	using System.Collections.Generic;

	public class SymbolFile : SourceFileBase,
		IComparable<SourceFile>,
		IEquatable<SourceFile>
	{
		public SymbolFile(string filename)
			: base(filename)
		{
			this.SourceFiles = new List<SourceFile>();
		}

		public List<SourceFile> SourceFiles { get; private set; }

		internal void AddSourceFile(SourceFile sourceFile)
		{
			if (sourceFile == null)
				throw new ArgumentNullException("sourceFile");

			if (!this.SourceFiles.Contains(sourceFile))
				this.SourceFiles.Add(sourceFile);
		}

		public int CompareTo(SourceFile other)
		{
			return base.CompareTo(other);
		}
		public bool Equals(SourceFile other)
		{
			return base.Equals(other);
		}
	}
}