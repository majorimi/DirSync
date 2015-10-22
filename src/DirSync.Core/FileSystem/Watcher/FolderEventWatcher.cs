using System.IO;
using DirSync.Core.Domain;
using DirSync.Core.Queue;

namespace DirSync.Core.FileSystem.Watcher
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