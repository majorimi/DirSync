using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirSyncService.Config
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
			get { return 10; }
		}


		static DirSyncConfiguration()
		{
			_sourceDir = new DirectoryInfo(ConfigurationManager.AppSettings["SourceDir"]);
			_targetDir = new DirectoryInfo(ConfigurationManager.AppSettings["TargetDir"]);
		}
	}
}
