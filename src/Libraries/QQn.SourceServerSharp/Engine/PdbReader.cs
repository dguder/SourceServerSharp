namespace QQn.SourceServerSharp.Engine
{
	using System.Collections.Generic;
	using System.Diagnostics;
	using Framework;

	internal static class PdbReader
	{
		public static void ReadSourceFilesFromPdbs(
			IndexerState state, string srcToolPath, bool reIndexPreviouslyIndexedSymbols)
		{
			List<SymbolFile> pdbsToRemove = null;
			foreach (var pdb in state.SymbolFiles.Values)
			{
				var psi = new ProcessStartInfo(srcToolPath)
				{
					WorkingDirectory = pdb.File.DirectoryName,
					UseShellExecute = false,
					RedirectStandardError = true,
					RedirectStandardOutput = true
				};

				string output;
				string errors;

				if (!reIndexPreviouslyIndexedSymbols)
				{
					psi.Arguments = string.Format("-c \"{0}\"", pdb.FullName);

					using (var p = Process.Start(psi))
					{
						output = p.StandardOutput.ReadToEnd();
						errors = p.StandardError.ReadToEnd();

						p.WaitForExit();
					}

					if (output.Contains("source files are indexed") ||
					    errors.Contains("source files are indexed") ||
					    output.Contains("source file is indexed") ||
					    errors.Contains("source file is indexed"))
					{
						// No need to change annotation; it is already indexed
						if (pdbsToRemove == null)
							pdbsToRemove = new List<SymbolFile>();

						pdbsToRemove.Add(pdb);
						continue;
					}
				}

				psi.Arguments = string.Format("-r \"{0}\"", pdb.FullName);

				using (var p = Process.Start(psi))
				{
					output = p.StandardOutput.ReadToEnd();
					errors = p.StandardError.ReadToEnd();

					p.WaitForExit();
				}

				if (!string.IsNullOrEmpty(errors))
					throw new SourceIndexToolException("SRCTOOL", errors.Trim());

				var foundOne = false;
				foreach (var item in output.Split('\r', '\n'))
				{
					var fileName = item.Trim();

					if (string.IsNullOrEmpty(fileName))
						continue; // We split on \r and \n

					if ((fileName.IndexOf('*') >= 0) || // C++ Compiler internal file
					    ((fileName.Length > 2) && (fileName.IndexOf(':', 2) >= 0)))
					{
						// Some compiler internal filenames of C++ start with a * 
						// and/or have a :123 suffix

						continue; // Skip never existing files
					}

					fileName = state.NormalizePath(fileName);

					SourceFile file;

					if (!state.SourceFiles.TryGetValue(fileName, out file))
					{
						file = new SourceFile(fileName);
						state.SourceFiles.Add(fileName, file);
					}

					pdb.AddSourceFile(file);
					file.AddContainer(pdb);
					foundOne = true;
				}

				if (foundOne)
					continue;

				if (pdbsToRemove == null)
					pdbsToRemove = new List<SymbolFile>();

				pdbsToRemove.Add(pdb);
			}

			if (pdbsToRemove == null)
				return;

			foreach (var s in pdbsToRemove)
				state.SymbolFiles.Remove(s.FullName);
		}
	}
}