namespace QQn.SourceServerSharp
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Engine;
	using Framework;
	using Providers;

	public class SourceServerIndexer
	{
		public SourceServerIndexer(Type provider)
		{
			this.SymbolFiles = new List<string>();
			this.SourceRoots = new List<string>();
			this.Providers = new List<string>(new[] { provider.FullName });
			this.Types = new List<string>();
			this.ResolverData = new SortedList<string, IndexerTypeData>(StringComparer.InvariantCultureIgnoreCase);
			this.SourceServerSdkDir = ".";
		}

		public IList<string> SymbolFiles { get; private set; }
		public IList<string> SourceRoots { get; private set; }
		public IList<string> Providers { get; private set; }
		public IList<string> Types { get; private set; }
		public IDictionary<string, IndexerTypeData> ResolverData { get; private set; }
		private string SourceServerSdkDir { get; set; }

		public IndexerResult Exec()
		{
			var state = new IndexerState();

			foreach (var symbolFile in this.SymbolFiles.Select(pdbFile => new SymbolFile(pdbFile)))
			{
				if (!symbolFile.Exists)
					throw new FileNotFoundException("Symbol file not found", symbolFile.FullName);

				state.SymbolFiles.Add(symbolFile.FullName, symbolFile);
			}

			this.ReadSourceFilesFromPdbs(state); // Check if there are files to index for this pdb file

			state.ResolverData = this.ResolverData;

			this.LoadProviders(state);
			ResolveFiles(state);

			this.WritePdbAnnotations(state);

			return CreateResultData(state);
		}

		private void ReadSourceFilesFromPdbs(IndexerState state)
		{
			PdbReader.ReadSourceFilesFromPdbs(state, this.SourceServerSdkDir, false);
		}
		private void LoadProviders(IndexerState state)
		{
			var providers = new List<SourceResolver>();
			foreach (var provider in this.Providers)
			{
				Type providerType;
				try
				{
					providerType = Type.GetType(provider, true, true);
				}
				catch (Exception e)
				{
					throw new SourceIndexException(string.Format("Can't load provider '{0}'", provider), e);
				}

				if (!typeof(SourceResolver).IsAssignableFrom(providerType) || providerType.IsAbstract)
					throw new SourceIndexException(string.Format("Provider '{0}' is not a valid SourceProvider", providerType.FullName));

				try
				{
					providers.Add((SourceResolver)Activator.CreateInstance(providerType, new object[] { state }));
				}
				catch (Exception e)
				{
					throw new SourceIndexException(string.Format("Can't initialize provider '{0}'", providerType.FullName), e);
				}
			}

			foreach (var sp in from sp in providers
			                   let detector = sp as ISourceProviderDetector
			                   where (detector != null) && !state.Resolvers.Contains(sp)
			                   where sp.Available && detector.CanProvideSources(state)
			                   select sp)
				state.Resolvers.Add(sp);
		}
		private static void ResolveFiles(IndexerState state)
		{
			foreach (var sp in state.Resolvers)
				sp.ResolveFiles();
		}
		private void WritePdbAnnotations(IndexerState state)
		{
			PdbWriter.WritePdbAnnotations(state, this.SourceServerSdkDir);
		}
		private static IndexerResult CreateResultData(IndexerState state)
		{
			var sources = state.SourceFiles.Values.Count(sf => sf.SourceReference != null);
			return new IndexerResult(true, state.SymbolFiles.Count, sources, state.Resolvers.Count);
		}
	}
}