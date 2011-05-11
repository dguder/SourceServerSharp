namespace QQn.SourceServerSharp.Providers
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Security.Cryptography;
	using Framework;

	public class NetworkShareResolver : SourceResolver, ISourceProviderDetector
	{
		private static readonly IDictionary<string, SourceReference> References =
			new Dictionary<string, SourceReference>();

		public NetworkShareResolver(IndexerState state)
			: base(state, "NetworkShare")
		{
		}
		public override void WriteEnvironment(StreamWriter writer)
		{
			writer.WriteLine("SRCSRVSRC=%published_artifacts_src%\\%var2%\\%fnfile%(%var1%)");
			writer.WriteLine("SRCSRVTRG=%targ%\\%var2%\\%fnfile%(%var1%)");
			writer.WriteLine("SRCSRVCMD=cmd.exe /c copy /y \"%srcsrvsrc%\" %srcsrvtrg%");
		}
		public override bool Available
		{
			get { return true; }
		}
		public bool CanProvideSources(IndexerState state)
		{
			return state.SourceFiles.Any(x => File.Exists(x.Key));
		}
		public override bool ResolveFiles()
		{
			foreach (var file in this.State.SourceFiles.Where(x => File.Exists(x.Key)))
				file.Value.SourceReference = this.ResolveReference(file.Key);

			return true;
		}
		private SourceReference ResolveReference(string filename)
		{
			SourceReference resolved;
			if (!References.TryGetValue(filename, out resolved))
				References[filename] = resolved = new NetworkShareSourceReference(this, filename);

			return resolved;
		}

		public override int SourceEntryVariableCount
		{
			get { return 2; }
		}
	}

	public class NetworkShareSourceReference : SourceReference
	{
		private readonly string filename;
		private readonly string hash;

		public NetworkShareSourceReference(SourceProvider provider, string filename)
			: base(provider)
		{
			this.filename = filename;
			this.hash = FormatHash(ComputeHash(filename));
			this.CopyToDestination();
		}
		private static byte[] ComputeHash(string filename)
		{
			using (HashAlgorithm algorithm = new SHA1Managed())
			using (Stream inputStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
				return algorithm.ComputeHash(inputStream);
		}
		private static string FormatHash(byte[] raw)
		{
			var hash = BitConverter.ToString(raw);
			hash = hash.ToLowerInvariant().Replace("-", "");
			return hash.Substring(0, 3) + "\\" + hash.Substring(3, 3) + "\\" + hash.Substring(6);
		}

		private void CopyToDestination()
		{
			var replicationPath = Environment.GetEnvironmentVariable("published_artifacts_src");
			if (string.IsNullOrEmpty(replicationPath))
				return;

			var destination = Path.Combine(replicationPath, hash, Path.GetFileName(filename) ?? string.Empty);
			if (File.Exists(destination))
				return;

			Console.WriteLine(string.Format("Replicating '{0}' to '{1}'.", this.filename, destination));
			var directory = Path.GetDirectoryName(destination) ?? string.Empty;
			Directory.CreateDirectory(directory);
			File.Copy(this.filename, destination, true);
		}

		public override string[] GetSourceEntries()
		{
			return new[] { this.filename, hash };
		}
	}
}