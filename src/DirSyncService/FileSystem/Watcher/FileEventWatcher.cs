﻿using DirSyncService.Queue;
using System.IO;
using DirSyncService.Domain;

namespace DirSyncService.FileSystem.Watcher
{
	public class FileEventWatcher : FileSystemEventWatcherBase
	{
		public FileEventWatcher(IConcurrentQueue<FileSystemEventQueueItem> eventQueue)
			: base(eventQueue)
		{ }

		public override void StartWatch(string path)
		{
			CreateFileWatcher(path, NotifyFilters.FileName | NotifyFilters.Size);
		}
	}
}