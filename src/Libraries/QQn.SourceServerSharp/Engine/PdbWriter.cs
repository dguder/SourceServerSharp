// **************************************************************************
// * $Id: PdbWriter.cs 39 2006-12-17 10:24:58Z bhuijben $
// * $HeadURL: http://sourceserversharp.googlecode.com/svn/trunk/src/Libraries/QQn.SourceServerSharp/Engine/PdbWriter.cs $
// **************************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using QQn.SourceServerSharp.Framework;
using System.IO;
using QQn.SourceServerSharp.Providers;
using System.Diagnostics;

namespace QQn.SourceServerSharp.Engine
{
	using System.Linq;

	static class PdbWriter
	{
		public static void WritePdbAnnotations(IndexerState state, string pdbStrPath)
		{
			foreach (SymbolFile file in state.SymbolFiles.Values)
			{
				Console.WriteLine("Indexing " + file.File.FullName);
				string tmpFile = Path.GetFullPath(Path.GetTempFileName());
				try
				{
					using (StreamWriter sw = File.CreateText(tmpFile))
					{
						if (!WriteAnnotations(state, file, sw))
							continue; // Temp file is deleted in finally
					}

					ProcessStartInfo psi = new ProcessStartInfo(pdbStrPath);
					psi.Arguments = string.Format("-w -s:srcsrv -p:\"{0}\" -i:\"{1}\"", file.FullName, tmpFile);
					psi.UseShellExecute = false;

					psi.RedirectStandardError = true;
					psi.RedirectStandardOutput = true;
					using (Process p = Process.Start(psi))
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

		static bool WriteAnnotations(IndexerState state, SymbolFile file, StreamWriter sw)
		{
			SortedList<string, SourceProvider> providers = new SortedList<string, SourceProvider>();
			int itemCount = 1;

			foreach (SourceFile sf in file.SourceFiles)
			{
				if (!sf.IsResolved || sf.NoSourceAvailable)
					continue;

				SourceReference sr = sf.SourceReference;
				SourceProvider provider = sr.SourceProvider;

				if (providers.ContainsKey(provider.Id))
					continue;

				providers.Add(provider.Id, provider);

				if (provider.SourceEntryVariableCount > itemCount)
					itemCount = provider.SourceEntryVariableCount;
			}

			if (providers.Count == 0)
				return false;

			sw.WriteLine("SRCSRV: ini ------------------------------------------------");
			sw.WriteLine("VERSION=1");
			sw.WriteLine("SRCSRV: variables ------------------------------------------");
			sw.WriteLine("DATETIME=" + DateTime.Now.ToUniversalTime().ToString("u"));
			foreach (SourceProvider sp in providers.Values)
			{
				sp.WriteEnvironment(sw);
			}
			sw.WriteLine("SRCSRV: source files ---------------------------------------");

			// Note: the sourcefile block must be written in the order they are found by the PdbReader
			//	otherwise SrcTool skips all sourcefiles which don't exist locally and are out of order
			foreach (var values in
				file.SourceFiles.Where(x => x.IsResolved && !x.NoSourceAvailable)
				.Select(sourceFile => sourceFile.SourceReference.GetSourceEntries()))
			{
				for (var i = 0; i < itemCount && i < values.Length; i++)
				{
					sw.Write(values[i]);

					if (i + 1 < values.Length)
						sw.Write('*');
				}

				sw.WriteLine();
			}

			sw.WriteLine("SRCSRV: end ------------------------------------------------");

			return true;
		}
	}
}