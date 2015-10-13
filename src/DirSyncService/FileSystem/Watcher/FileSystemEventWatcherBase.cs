using DirSyncService.Queue;
using System.IO;
using DirSyncService.Domain;
using DirSyncService.Logging;

namespace DirSyncService.FileSystem.Watcher
{
	public abstract class FileSystemEventWatcherBase
	{
		public IConcurrentQueue<FileSystemEventQueueItem> Changes { get; private set; }

		public FileSystemEventWatcherBase(IConcurrentQueue<FileSystemEventQueueItem> eventQueue)
		{
			Changes = eventQueue;
		}

		public abstract void StartWatch(string path);

		protected void CreateFileWatcher(string path,
			NotifyFilters filters)
		{
			FileSystemWatcher fsw = new FileSystemWatcher(path);
			fsw.IncludeSubdirectories = true;
			fsw.EnableRaisingEvents = true;
			fsw.NotifyFilter = filters;
			fsw.InternalBufferSize = 20000;

			fsw.Changed += FileSytemEvents;
			fsw.Created += FileSytemEvents;
			fsw.Deleted += FileSytemEvents;
			fsw.Renamed += FileSytemEvents;

			fsw.Error += FileSystemWatcherError;
		}

		private void FileSytemEvents(object sender, FileSystemEventArgs e)
		{
			Changes.Enqueue(new FileSystemEventQueueItem(e));
		}

		private void FileSystemWatcherError(object sender, ErrorEventArgs e)
		{
			Logger.Current.Fatal($"An error occurred in File System Watcher: {this.GetType().Name} may be file system event lost. Exception: {e.GetException()}");
        }
	}
}
