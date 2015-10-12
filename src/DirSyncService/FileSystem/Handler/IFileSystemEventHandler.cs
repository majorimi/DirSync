using DirSyncService.FileSystem.Watcher;
using DirSyncService.Queue;

namespace DirSyncService.FileSystem.Handler
{
    public interface IFileSystemEventHandler
    {
		FileSystemEventWatcherBase EventWatcher { get; }

		IConcurrentQueue<FileSystemErrorEventQueueItem> ProcessErrors { get; }

		void Process();
    }
}
