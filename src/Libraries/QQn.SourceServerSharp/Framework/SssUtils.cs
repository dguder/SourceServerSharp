using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace QQn.SourceServerSharp.Framework
{
	static class SssUtils
	{
		public static string FindExecutable(string name)
		{
			string systemPath = Environment.GetEnvironmentVariable("PATH");

			if (systemPath == null)
				systemPath = "";

			// Search all directories in path
			foreach (string pathItem in systemPath.Split(Path.PathSeparator))
			{
				string item = pathItem.Trim();

				if (!Directory.Exists(item))
					continue;

				string file = Path.GetFullPath(Path.Combine(item, name));

				if (File.Exists(file))
					return Path.GetFullPath(file);
			}

			return null;
		}
	}
}
