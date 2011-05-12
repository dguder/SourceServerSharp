namespace QQn.SourceServerSharp.Providers
{
	using System;
	using System.IO;
	using Framework;

	public abstract class SourceProvider
	{
		private readonly IndexerState state;

		protected SourceProvider(IndexerState state, string name)
		{
			if (state == null)
				throw new ArgumentNullException("state");

			this.state = state;
			this.Id = state.AssignId(this, name);
		}

		public string Id { get; private set; }
		public virtual string Name
		{
			get { return null; }
		}

		protected IndexerState State
		{
			get { return this.state; }
		}

		public abstract int SourceEntryVariableCount { get; }
		public abstract void WriteEnvironment(StreamWriter writer);
	}
}