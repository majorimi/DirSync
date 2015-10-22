using System.IO;
using DirSync.Core.Domain;
using DirSync.Core.Queue;

namespace DirSync.Core.FileSystem.Watcher
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