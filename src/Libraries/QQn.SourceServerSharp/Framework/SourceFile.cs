namespace QQn.SourceServerSharp.Framework
{
	using System;
	using System.Collections.Generic;
	using Providers;

	public class SourceFile : SourceFileBase,
		IComparable<SourceFile>,
		IEquatable<SourceFile>
	{
		private readonly SortedList<string, SymbolFile> symbolFiles =
			new SortedList<string, SymbolFile>(StringComparer.InvariantCultureIgnoreCase);
		public bool SourceAvailable { get; set; }
		public SourceReference SourceReference { get; set; }

		public SourceFile(string filename)
			: base(filename)
		{
			this.SourceAvailable = true;
		}

		public IList<SymbolFile> Containers
		{
			get { return this.symbolFiles.Values; }
		}
		public bool IsResolved
		{
			get { return this.SourceReference != null && this.SourceAvailable; }
		}
		internal void AddContainer(SymbolFile symbolFile)
		{
			if (symbolFile == null)
				throw new ArgumentNullException("symbolFile");

			if (!this.symbolFiles.ContainsKey(symbolFile.FullName))
				this.symbolFiles.Add(symbolFile.FullName, symbolFile);
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