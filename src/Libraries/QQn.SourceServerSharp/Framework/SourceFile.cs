// **************************************************************************
// * $Id: SourceFile.cs 39 2006-12-17 10:24:58Z bhuijben $
// * $HeadURL: http://sourceserversharp.googlecode.com/svn/trunk/src/Libraries/QQn.SourceServerSharp/Framework/SourceFile.cs $
// **************************************************************************

namespace QQn.SourceServerSharp.Framework
{
	using System;
	using System.Collections.Generic;
	using Providers;

	/// <summary>
	/// 
	/// </summary>
	public class SourceFile : SourceFileBase,
		IComparable<SourceFile>,
		IEquatable<SourceFile>
	{
		private readonly SortedList<string, SymbolFile> symbolFiles =
			new SortedList<string, SymbolFile>(StringComparer.InvariantCultureIgnoreCase);

		/// <summary>
		/// Creates a new SourceFile object for the specified file
		/// </summary>
		/// <param name="filename"></param>
		public SourceFile(string filename)
			: base(filename)
		{
			this.SourceAvailable = true;
		}

		/// <summary>
		/// Gets tje list of symbolfiles containing a reference to this sourcefile
		/// </summary>
		public IList<SymbolFile> Containers
		{
			get { return this.symbolFiles.Values; }
		}

		/// <summary>
		/// Gets or sets the <see cref="SourceReference"/> for this <see cref="SourceFile"/>
		/// </summary>
		public SourceReference SourceReference { get; set; }

		/// <summary>
		/// Gets a value indicating whether a source-reference has been found
		/// </summary>
		/// <value><c>true</c> when a <see cref="SourceReference"/> is available or when <see cref="SourceAvailable"/> is set, otherwise <c>false</c></value>
		public bool IsResolved
		{
			get { return this.SourceReference != null && this.SourceAvailable; }
		}

		public bool SourceAvailable { get; set; }
		
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