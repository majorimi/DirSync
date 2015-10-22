using System;
using System.Configuration;
using System.IO;

namespace DirSync.Core.Config
{
	public static class DirSyncConfiguration
	{
		private static readonly DirectoryInfo _sourceDir;
		private static readonly DirectoryInfo _targetDir;

		public static bool IsBackUpMode
		{
			get
			{
				return Convert.ToBoolean(ConfigurationManager.AppSettings["BackUpMode"]);
			}
		}

		public static string BackUpDirName
		{
			get
			{
				return ConfigurationManager.AppSettings["BackUpDirName"];
			}
		}

		public static DirectoryInfo SourceDir
		{
			get
			{
				return _sourceDir;
			}
		}

		public static DirectoryInfo TargetDir
		{
			get
			{
				return _targetDir;
			}
		}

		public static int MaxTrying
		{
			get { return Convert.ToInt32(ConfigurationManager.AppSettings["MaximumRetry"]); }
		}


		static DirSyncConfiguration()
		{
			_sourceDir = new DirectoryInfo(ConfigurationManager.AppSettings["SourceDir"]);
			_targetDir = new DirectoryInfo(ConfigurationManager.AppSettings["TargetDir"]);
		}
	}
}
