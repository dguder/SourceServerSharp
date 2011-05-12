namespace QQn.SourceServerSharp.Framework
{
	using System;
	using System.IO;
	using System.Linq;

	internal static class SssUtils
	{
		public static string FindExecutable(string name)
		{
			var systemPath = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;

			return (from pathItem in systemPath.Split(Path.PathSeparator)
			        select pathItem.Trim()
			        into item where Directory.Exists(item) select Path.GetFullPath(Path.Combine(item, name))
			        into file where File.Exists(file) select Path.GetFullPath(file)).FirstOrDefault();
		}
	}
}