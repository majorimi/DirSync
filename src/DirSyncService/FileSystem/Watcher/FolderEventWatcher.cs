using DirSyncService.Queue;
using System.IO;
using DirSyncService.Domain;

namespace DirSyncService.FileSystem.Watcher
{
	public class FolderEventWatcher : FileSystemEventWatcherBase
	{
		public FolderEventWatcher(IConcurrentQueue<FileSystemEventQueueItem> eventQueue)
			: base(eventQueue)
		{ }

		public override void StartWatch(string path)
		{
			CreateFileWatcher(path, NotifyFilters.DirectoryName);
		}
	}
}