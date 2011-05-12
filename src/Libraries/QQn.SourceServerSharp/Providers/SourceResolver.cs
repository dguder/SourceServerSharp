namespace QQn.SourceServerSharp.Providers
{
	using Framework;

	public interface ISourceProviderDetector
	{
		bool CanProvideSources(IndexerState state);
	}

	public abstract class SourceResolver : SourceProvider
	{
		private readonly string name;

		protected SourceResolver(IndexerState state, string name)
			: base(state, name)
		{
			this.name = name;
		}

		public override string Name
		{
			get { return this.name; }
		}
		public abstract bool Available { get; }
		public abstract bool ResolveFiles();
		public override int SourceEntryVariableCount
		{
			get { return 2; }
		}
	}
}