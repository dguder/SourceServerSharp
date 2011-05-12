namespace SssIndex
{
	using QQn.SourceServerSharp;
	using QQn.SourceServerSharp.Providers;

	internal class Program
	{
		private static void Main(string[] args)
		{
			new SourceServerIndexer(typeof(NetworkShareResolver)).Exec();
		}
	}
}