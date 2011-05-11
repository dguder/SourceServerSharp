// **************************************************************************
// * $Id: SourceServerIndexer.cs 57 2008-02-09 22:42:34Z bhuijben $
// * $HeadURL: http://sourceserversharp.googlecode.com/svn/trunk/src/Libraries/QQn.SourceServerSharp/SourceServerIndexer.cs $
// **************************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using QQn.SourceServerSharp.Framework;
using System.Diagnostics;
using QQn.SourceServerSharp.Providers;
using QQn.SourceServerSharp.Engine;
using Microsoft.Win32;

namespace QQn.SourceServerSharp
{
	/// <summary>
	/// 
	/// </summary>
	public class SourceServerIndexer
	{
		IList<string> _symbolFiles = new List<string>();
		IList<string> _sourceRoots = new List<string>();
		IList<string> _excludeSourceRoots = new List<string>();

		IList<string> _providerTypes = new List<string>(new string[]
			{
				typeof(NetworkShareResolver).FullName
			});

		IList<string> _srcTypes = new List<string>(new string[] { "autodetect" });
		IDictionary<string, IndexerTypeData> _indexerData = new SortedList<string, IndexerTypeData>(StringComparer.InvariantCultureIgnoreCase);

		string _sourceServerSdkDir = null;
		string _registrySourceServerSdkDir = null;
		bool _reindexPreviouslyIndexed;

		/// <summary>
		/// Initializes a new SourceServerIndexer
		/// </summary>
		public SourceServerIndexer()
		{
			using (RegistryKey rk = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\DebuggingTools", false))
			{
				if (rk != null)
				{
					string path = rk.GetValue("WinDbg") as string;

					if (path != null)
					{
						path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar).TrimEnd(Path.DirectorySeparatorChar);

						if (Directory.Exists(path))
						{
							string srcSdkPath = Path.Combine(path, "sdk\\srcsrv");

							if (Directory.Exists(path))
							{
								SourceServerSdkDir = _registrySourceServerSdkDir = Path.GetFullPath(srcSdkPath);
								return;
							}
						}
					}
				}
			}

			string dir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

			string sdkDir = Path.Combine(dir, "Debugging Tools for Windows\\sdk\\srcsrv");
			if(Directory.Exists(sdkDir))
				SourceServerSdkDir = sdkDir;


			sdkDir = Path.Combine(dir, "Debugging Tools for Windows 64-bit\\sdk\\srcsrv");
			if (Directory.Exists(sdkDir))
				SourceServerSdkDir = sdkDir;
		}

		/// <summary>
		/// Gets or sets a list of symbol files to index
		/// </summary>
		public IList<string> SymbolFiles
		{
			get { return _symbolFiles; }
			set
			{
				if (value != null)
					_symbolFiles = value;
				else
					_symbolFiles = new string[0];
			}
		}

		/// <summary>
		/// Gets or sets a list of sourcecode directories to index
		/// </summary>
		/// <remarks>If one or more sourceroots are specified, only files in and below these directories are indexed</remarks>
		public IList<string> SourceRoots
		{
			get { return _sourceRoots; }
			set
			{
				if (value != null)
					_sourceRoots = value;
				else
					_sourceRoots = new string[0];
			}
		}

		/// <summary>
		/// Gets or sets a list of sourcecode directories not to index
		/// </summary>
		/// <remarks>These directories allow to exclude specific directories which are included in the <see cref="SourceRoots"/></remarks>
		public IList<string> ExcludeSourceRoots
		{
			get { return _sourceRoots; }
			set
			{
				if (value != null)
					_excludeSourceRoots = value;
				else
					_excludeSourceRoots = new string[0];
			}
		}

		/// <summary>
		/// Gets or sets a list of sourcecode provider types
		/// </summary>
		public IList<string> Providers
		{
			get { return _providerTypes; }
			set
			{
				if (value != null)
					_providerTypes = value;
				else
					_providerTypes = new string[0];
			}
		}

		/// <summary>
		/// Gets or sets a list of sourcecode directories to index
		/// </summary>
		/// <remarks>If one or more sourceroots are specified, only files in and below these directories are indexed</remarks>
		public IList<string> Types
		{
			get { return _srcTypes; }
			set
			{
				if (value != null)
					_srcTypes = value;
				else
					_srcTypes = new string[0];
			}
		}

		/// <summary>
		/// Gets or sets a dictionary containing source information per path
		/// </summary>
		public IDictionary<string, IndexerTypeData> ResolverData
		{
			get { return _indexerData; }
			set
			{
				if (value != null)
					_indexerData = value;
				else
					_indexerData = new SortedList<string, IndexerTypeData>(StringComparer.InvariantCultureIgnoreCase);
			}
		}

		/// <summary>
		/// Gets or sets the SourceServer SDK directory
		/// </summary>
		public string SourceServerSdkDir
		{
			get { return _sourceServerSdkDir; }
			set
			{
				if (value != null)
					_sourceServerSdkDir = value;
				else
					value = null;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public bool ReIndexPreviouslyIndexedSymbols
		{
			get { return _reindexPreviouslyIndexed; }
			set { _reindexPreviouslyIndexed = value; }
		}

		string _srcToolPath;
		string _pdbStrPath;

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IndexerResult Exec()
		{
			string sdkDir = SourceServerSdkDir;

			if(string.IsNullOrEmpty(sdkDir))
			{
				Uri codeBase = new Uri(typeof(SourceServerIndexer).Assembly.CodeBase);

				if(codeBase.IsFile)
					sdkDir = Path.GetDirectoryName(codeBase.LocalPath);
			}

			if(string.IsNullOrEmpty(_srcToolPath) || !File.Exists(_srcToolPath))
			{
				string path;

				if(!string.IsNullOrEmpty(sdkDir) && Directory.Exists(sdkDir))
				{
					if(File.Exists(path = Path.Combine(sdkDir, "srctool.exe")))
						_srcToolPath = Path.GetFullPath(path);
				}

				if (string.IsNullOrEmpty(_srcToolPath) && !string.IsNullOrEmpty(_registrySourceServerSdkDir) && Directory.Exists(_registrySourceServerSdkDir))
				{
					if (File.Exists(path = Path.Combine(_registrySourceServerSdkDir, "srctool.exe")))
						_srcToolPath = Path.GetFullPath(path);
				}

				if(string.IsNullOrEmpty(_srcToolPath))
					_srcToolPath = SssUtils.FindExecutable("srctool.exe");

				if(string.IsNullOrEmpty(_srcToolPath))
					throw new FileNotFoundException("SRCTOOL.EXE not found", "srctool.exe");
			}

			if(string.IsNullOrEmpty(_pdbStrPath) || !File.Exists(_pdbStrPath))
			{
				string path;

				if(!string.IsNullOrEmpty(sdkDir) && Directory.Exists(sdkDir))
				{
					if(File.Exists(path = Path.Combine(sdkDir, "pdbstr.exe")))
						_pdbStrPath = Path.GetFullPath(path);
				}

				if (!string.IsNullOrEmpty(_registrySourceServerSdkDir) && Directory.Exists(_registrySourceServerSdkDir))
				{
					if (File.Exists(path = Path.Combine(_registrySourceServerSdkDir, "pdbstr.exe")))
						_pdbStrPath = Path.GetFullPath(path);
				}

				if(string.IsNullOrEmpty(_pdbStrPath))
					_pdbStrPath = SssUtils.FindExecutable("pdbstr.exe");

				if(string.IsNullOrEmpty(_srcToolPath))
					throw new FileNotFoundException("PDBSTR.EXE not found", "pdbstr.exe");
			}


			IndexerState state = new IndexerState();

			foreach (string pdbFile in SymbolFiles)
			{
				SymbolFile symbolFile = new SymbolFile(pdbFile);

				if (!symbolFile.Exists)
					throw new FileNotFoundException("Symbol file not found", symbolFile.FullName);

				state.SymbolFiles.Add(symbolFile.FullName, symbolFile);
			}

			ReadSourceFilesFromPdbs(state); // Check if there are files to index for this pdb file

			PerformExclusions(state);

			state.ResolverData = this.ResolverData;

			LoadProviders(state);
			ResolveFiles(state);

			WritePdbAnnotations(state);

			return CreateResultData(state);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="state"></param>
		void ReadSourceFilesFromPdbs(IndexerState state)
		{
			PdbReader.ReadSourceFilesFromPdbs(state, _srcToolPath, ReIndexPreviouslyIndexedSymbols);
		}

		void PerformExclusions(IndexerState state)
		{
			#region - Apply SourceRoots
			if (SourceRoots.Count > 0)
			{
				List<string> rootList = new List<string>();

				foreach (string root in SourceRoots)
				{
					string nRoot = state.NormalizePath(root);

					if(!nRoot.EndsWith("\\"))
						nRoot += "\\";

					rootList.Add(nRoot);
				}

				string[] roots = rootList.ToArray();
				Array.Sort<string>(roots, StringComparer.InvariantCultureIgnoreCase);

				foreach (SourceFile sf in state.SourceFiles.Values)
				{
					string fileName = sf.FullName;

					int n = Array.BinarySearch<string>(roots, fileName, StringComparer.InvariantCultureIgnoreCase);

					if (n >= 0)
						continue; // Exact match found

					n = ~n;

					if ((n > 0) && (n <= roots.Length))
					{
						if (fileName.StartsWith(roots[n - 1], StringComparison.InvariantCultureIgnoreCase))
							continue; // Root found

						sf.NoSourceAvailable = true;
					}
					else
						sf.NoSourceAvailable = true;
				}
			}
			#endregion - Apply SourceRoots
			#region - Apply ExcludeSourceRoots
			if (ExcludeSourceRoots.Count > 0)
			{
				List<string> rootList = new List<string>();

				foreach (string root in ExcludeSourceRoots)
				{
					string nRoot = state.NormalizePath(root);

					if (!nRoot.EndsWith(Path.DirectorySeparatorChar.ToString()))
						nRoot += Path.DirectorySeparatorChar;

					rootList.Add(nRoot);
				}

				string[] roots = rootList.ToArray();
				Array.Sort<string>(roots, StringComparer.InvariantCultureIgnoreCase);

				foreach (SourceFile sf in state.SourceFiles.Values)
				{
					string fileName = sf.FullName;

					int n = Array.BinarySearch<string>(roots, fileName, StringComparer.InvariantCultureIgnoreCase);

					if (n >= 0)
						continue; // Exact match found

					n = ~n;

					if ((n > 0) && (n <= roots.Length))
					{
						if (fileName.StartsWith(roots[n - 1], StringComparison.InvariantCultureIgnoreCase))
							sf.NoSourceAvailable = true;
					}
				}
			}
			#endregion


		}

		void LoadProviders(IndexerState state)
		{
			List<SourceResolver> providers = new List<SourceResolver>();
			foreach(string provider in Providers)
			{
				Type providerType;
				try
				{
					providerType = Type.GetType(provider, true, true);
				}
				catch(Exception e)
				{
					throw new SourceIndexException(string.Format("Can't load provider '{0}'", provider), e);
				}

				if (!typeof(SourceResolver).IsAssignableFrom(providerType) || providerType.IsAbstract)
					throw new SourceIndexException(string.Format("Provider '{0}' is not a valid SourceProvider", providerType.FullName));

				try
				{
					providers.Add((SourceResolver)Activator.CreateInstance(providerType, new object[] { state }));
				}
				catch (Exception e)
				{
					throw new SourceIndexException(string.Format("Can't initialize provider '{0}'", providerType.FullName), e);
				}
			}

			foreach (SourceResolver sp in providers)
			{
				var detector = sp as ISourceProviderDetector;

				if ((detector != null) && !state.Resolvers.Contains(sp))
				{
					if (sp.Available && detector.CanProvideSources(state))
						state.Resolvers.Add(sp);
				}
			}
		}

		void ResolveFiles(IndexerState state)
		{
			foreach (SourceResolver sp in state.Resolvers)
			{
				sp.ResolveFiles();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="state"></param>
		void WritePdbAnnotations(IndexerState state)
		{
			PdbWriter.WritePdbAnnotations(state, _pdbStrPath);
		}

		IndexerResult CreateResultData(IndexerState state)
		{
			int nSources = 0;

			foreach (SourceFile sf in state.SourceFiles.Values)
			{
				if (sf.SourceReference != null)
					nSources++;
			}

			return new IndexerResult(true, state.SymbolFiles.Count, nSources, state.Resolvers.Count);
		}
	}
}
