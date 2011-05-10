// **************************************************************************
// * $Id: IndexerState.cs 40 2006-12-17 12:54:18Z bhuijben $
// * $HeadURL: http://sourceserversharp.googlecode.com/svn/trunk/src/Libraries/QQn.SourceServerSharp/Framework/IndexerState.cs $
// **************************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using QQn.SourceServerSharp.Framework;
using QQn.SourceServerSharp.Providers;
using System.IO;

namespace QQn.SourceServerSharp.Framework
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class IndexerState
	{
		readonly SortedList<string, SymbolFile> _symbolFiles = new SortedList<string,SymbolFile>(StringComparer.InvariantCultureIgnoreCase);
		readonly SortedList<string, SourceFile> _sourceFiles = new SortedList<string, SourceFile>(StringComparer.InvariantCultureIgnoreCase);
		readonly SortedList<string, SourceProvider> _srcProviders = new SortedList<string, SourceProvider>(StringComparer.InvariantCultureIgnoreCase);
		readonly List<SourceResolver> _resolvers = new List<SourceResolver>();
		IDictionary<string, IndexerTypeData> _resolverData = new SortedList<string, IndexerTypeData>(StringComparer.InvariantCultureIgnoreCase);

		/// <summary>
		/// 
		/// </summary>
		public IndexerState()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		public SortedList<string, SymbolFile> SymbolFiles
		{
			get { return _symbolFiles; }
		}

		/// <summary>
		/// 
		/// </summary>
		public SortedList<string, SourceFile> SourceFiles
		{
			get { return _sourceFiles; }
		}


		/// <summary>
		/// 
		/// </summary>
		public IDictionary<string, IndexerTypeData> ResolverData
		{
			get { return _resolverData; }
			set 
			{ 
				if(value != null)
					_resolverData = value; 
				else
					_resolverData = new SortedList<string, IndexerTypeData>(StringComparer.InvariantCultureIgnoreCase);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public List<SourceResolver> Resolvers
		{
			get { return _resolvers; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public string NormalizePath(string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

			if (!Path.IsPathRooted(path) || path.Contains(".."))
				path = Path.GetFullPath(path);

			return path;
		}

		static string SafeId(string name)
		{
			StringBuilder sb = new StringBuilder();

			if (name.Length > 0)
			{
				if (!char.IsLetter(name, 0))
					sb.Append("PRV");
				else
					sb.Append(name[0]);

				for (int i = 1; i < name.Length; i++)
					if (char.IsLetterOrDigit(name, i))
						sb.Append(char.ToUpperInvariant(name[i]));
			}
			else
				return "PRV";
			
			return sb.ToString();
		}

		internal string AssignId(SourceProvider sp, string name)
		{
			if (_srcProviders.ContainsValue(sp))
				return sp.Id;

			string id = SafeId(name);

			if (_srcProviders.ContainsKey(name))
			{
				string tmpId;
				int i=0;
				do
				{
					tmpId = string.Format("{0}{1:X}", id, i++);
				}
				while (_srcProviders.ContainsKey(tmpId));

				id = tmpId;
			}

			_srcProviders.Add(id, sp);

			return id;
		}
	}
}
