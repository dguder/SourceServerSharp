namespace QQn.SourceServerSharp.Framework
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using Providers;

	public sealed class IndexerState
	{
		private readonly SortedList<string, SourceProvider> srcProviders = new SortedList<string, SourceProvider>(StringComparer.InvariantCultureIgnoreCase);
		private IDictionary<string, IndexerTypeData> resolverData = new SortedList<string, IndexerTypeData>(StringComparer.InvariantCultureIgnoreCase);

		public IndexerState()
		{
			this.Resolvers = new List<SourceResolver>();
			this.SourceFiles = new SortedList<string, SourceFile>(StringComparer.InvariantCultureIgnoreCase);
			this.SymbolFiles = new SortedList<string, SymbolFile>(StringComparer.InvariantCultureIgnoreCase);
		}

		public SortedList<string, SymbolFile> SymbolFiles { get; private set; }
		public SortedList<string, SourceFile> SourceFiles { get; private set; }
		public List<SourceResolver> Resolvers { get; private set; }
		public IDictionary<string, IndexerTypeData> ResolverData
		{
			get { return this.resolverData; }
			set { this.resolverData = value ?? new SortedList<string, IndexerTypeData>(StringComparer.InvariantCultureIgnoreCase); }
		}

		public string NormalizePath(string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");

			path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

			if (!Path.IsPathRooted(path) || path.Contains(".."))
				path = Path.GetFullPath(path);

			return path;
		}
		private static string SafeId(string name)
		{
			var sb = new StringBuilder();

			if (name.Length > 0)
			{
				if (!char.IsLetter(name, 0))
					sb.Append("PRV");
				else
					sb.Append(name[0]);

				for (var i = 1; i < name.Length; i++)
				{
					if (char.IsLetterOrDigit(name, i))
						sb.Append(char.ToUpperInvariant(name[i]));
				}
			}
			else
				return "PRV";

			return sb.ToString();
		}
		internal string AssignId(SourceProvider sp, string name)
		{
			if (this.srcProviders.ContainsValue(sp))
				return sp.Id;

			var id = SafeId(name);

			if (this.srcProviders.ContainsKey(name))
			{
				string tmpId;
				var i = 0;
				do
				{
					tmpId = string.Format("{0}{1:X}", id, i++);
				}
				while (this.srcProviders.ContainsKey(tmpId));

				id = tmpId;
			}

			this.srcProviders.Add(id, sp);

			return id;
		}
	}
}