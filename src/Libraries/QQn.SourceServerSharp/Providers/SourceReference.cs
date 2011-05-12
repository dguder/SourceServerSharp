namespace QQn.SourceServerSharp.Providers
{
	using System;

	public abstract class SourceReference
	{
		protected SourceReference(SourceProvider provider)
		{
			if (provider == null)
				throw new ArgumentNullException("provider");

			this.SourceProvider = provider;
		}
		public SourceProvider SourceProvider { get; private set; }
		public abstract string[] GetSourceEntries();
	}
}