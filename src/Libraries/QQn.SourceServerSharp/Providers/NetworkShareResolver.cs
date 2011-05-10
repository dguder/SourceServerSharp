namespace QQn.SourceServerSharp.Providers
{
	using System;
	using System.IO;
	using System.Security.Cryptography;
	using Framework;

	public class NetworkShareResolver : SourceResolver,
		ISourceProviderDetector
	{
		public NetworkShareResolver(IndexerState state)
			: base(state, "NetworkShare")
		{
		}
		public override void WriteEnvironment(StreamWriter writer)
		{
			writer.WriteLine("SRCSRVSRC=\"%artifact_src_path%\\%var2%\\%fnfile%(%var1%)\"");
			writer.WriteLine("SRCSRVTRG=%targ%\\%var2%\\%fnfile%(%var1%)");
			writer.WriteLine("SRCSRVCMD=cmd.exe /c copy /y %srcsrvsrc% %srcsrvtrg%");
		}
		public override bool Available
		{
			get { return true; }
		}
		public bool CanProvideSources(IndexerState state)
		{
			return true;
		}
		public override bool ResolveFiles()
		{
			foreach (var file in this.State.SourceFiles)
				file.Value.SourceReference = new NetworkShareSourceReference(this, file.Key);

			return true;
		}
		public override int SourceEntryVariableCount
		{
			get { return 2; }
		}
	}

	public class NetworkShareSourceReference : SourceReference
	{
		private readonly string outputPath;
		private readonly string filename;
		private readonly string hash;

		public NetworkShareSourceReference(SourceProvider provider, string filename)
			: base(provider)
		{
			this.filename = filename;
			this.hash = FormatHash(ComputeHash(filename));

			var replicationPath = Environment.GetEnvironmentVariable("artifact_src_path");
			if (!string.IsNullOrEmpty(replicationPath))
				this.outputPath = Path.Combine(replicationPath, hash, Path.GetFileName(filename) ?? string.Empty);
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

		public override string[] GetSourceEntries()
		{
			Replicate();

			return new[] { this.filename, hash };
		}
		private void Replicate()
		{
			if (string.IsNullOrEmpty(this.outputPath))
				return;

			var directory = Path.GetDirectoryName(this.outputPath) ?? string.Empty;
			Directory.CreateDirectory(directory);
			if (File.Exists(this.outputPath))
				return;

			Console.WriteLine(string.Format("Replicating '{0}' to '{1}'.", this.filename, this.outputPath));
			File.Copy(this.filename, this.outputPath, true);
		}
	}
}