using System;
using System.IO;

namespace DirSyncService.Domain
{
	public class FileSystemChangeEvent
	{
		public WatcherChangeTypes ChangeType { get; set; }

		public string FullPath { get; set; }

		public string Name { get; set; }

		public DateTime EventTime { get; set; }

		/// <summary>
		/// Default constructor for serialize only!
		/// </summary>
		public FileSystemChangeEvent()
		{}

		public static FileSystemChangeEvent Create(FileSystemEventArgs eventArgs)
		{
			FileSystemChangeEvent ret;

			if (eventArgs.ChangeType == WatcherChangeTypes.Renamed)
			{
				RenamedEventArgs renamedEventArgs = (RenamedEventArgs)eventArgs;
				ret = new FileSystemRenameEvent()
				{
					OldFullPath = renamedEventArgs.OldFullPath,
					OldName = renamedEventArgs.OldName,
				};
			}
			else
			{
				ret = new FileSystemChangeEvent();
			}

			ret.ChangeType = eventArgs.ChangeType;
			ret.Name = eventArgs.Name;
			ret.FullPath = eventArgs.FullPath;
			ret.EventTime = DateTime.Now;

			return ret;
		}
	}
}
