// **************************************************************************
// * $Id: PdbWriter.cs 39 2006-12-17 10:24:58Z bhuijben $
// * $HeadURL: http://sourceserversharp.googlecode.com/svn/trunk/src/Libraries/QQn.SourceServerSharp/Engine/PdbWriter.cs $
// **************************************************************************

namespace QQn.SourceServerSharp.Engine
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using Framework;
	using Providers;

	internal static class PdbWriter
	{
		public static void WritePdbAnnotations(IndexerState state, string pdbStrPath)
		{
			foreach (var file in state.SymbolFiles.Values)
			{
				var tmpFile = Path.GetFullPath(Path.GetTempFileName());
				try
				{
					using (var sw = File.CreateText(tmpFile))
					{
						if (!WriteAnnotations(file, sw))
							continue; // Temp file is deleted in finally
					}

					var psi = new ProcessStartInfo(pdbStrPath)
					{
						Arguments = string.Format("-w -s:srcsrv -p:\"{0}\" -i:\"{1}\"", file.FullName, tmpFile),
						UseShellExecute = false,
						RedirectStandardError = true,
						RedirectStandardOutput = true
					};

					using (var p = Process.Start(psi))
					{
						p.StandardOutput.ReadToEnd();
						p.StandardError.ReadToEnd();

						p.WaitForExit();
					}
				}
				finally
				{
					File.Delete(tmpFile);
				}
			}
		}

		private static bool WriteAnnotations(SymbolFile file, StreamWriter sw)
		{
			var providers = new SortedList<string, SourceProvider>();
			var itemCount = 1;

			var available = file.SourceFiles
				.Where(x => x.IsResolved)
				.Select(x => x.SourceReference.SourceProvider)
				.Where(x => !providers.ContainsKey(x.Id));

			foreach (var provider in available)
			{
				providers.Add(provider.Id, provider);
				if (provider.SourceEntryVariableCount > itemCount)
					itemCount = provider.SourceEntryVariableCount;
			}

			if (providers.Count == 0)
				return false;

			Console.WriteLine("Indexing " + file.File.FullName);
			sw.WriteLine("SRCSRV: ini ------------------------------------------------");
			sw.WriteLine("VERSION=1");
			sw.WriteLine("SRCSRV: variables ------------------------------------------");
			sw.WriteLine("DATETIME=" + DateTime.UtcNow.ToString("u"));
			foreach (var sp in providers.Values)
				sp.WriteEnvironment(sw);
			sw.WriteLine("SRCSRV: source files ---------------------------------------");

			var files = file.SourceFiles
				.Where(x => x.IsResolved)
				.Select(x => x.SourceReference.GetSourceEntries());

			foreach (var values in files)
				sw.WriteLine(string.Join("*", values.Take(itemCount)));

			sw.WriteLine("SRCSRV: end ------------------------------------------------");

			return true;
		}
	}
}