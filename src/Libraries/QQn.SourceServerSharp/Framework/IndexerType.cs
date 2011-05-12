namespace QQn.SourceServerSharp.Framework
{
	using System;

	public class IndexerTypeData
	{
		public IndexerTypeData(string path, string type, string info)
		{
			if (path == null)
				throw new ArgumentNullException("path");
			if (type == null)
				throw new ArgumentNullException("type");

			this.Path = path;
			this.Type = type;
			this.Info = string.IsNullOrEmpty(info) ? string.Empty : info;
		}

		public string Path { get; private set; }
		public string Type { get; private set; }
		public string Info { get; private set; }
	}
}