namespace QQn.SourceServerSharp
{
	public class IndexerResult
	{
		public bool Success { get; private set; }
		public int IndexedSymbolFiles { get; private set; }
		public int IndexedSourceFiles { get; private set; }
		public int ProvidersUsed { get; private set; }
		public string ErrorMessage { get; private set; }

		internal IndexerResult(bool success, int indexedSymbolFiles, int indexedSourceFiles, int providersUsed)
		{
			this.Success = success;
			this.IndexedSymbolFiles = indexedSymbolFiles;
			this.IndexedSourceFiles = indexedSourceFiles;
			this.ProvidersUsed = providersUsed;
		}
		internal IndexerResult(bool success, string errorMessage)
		{
			this.Success = success;
			this.ErrorMessage = errorMessage;
		}
	}
}