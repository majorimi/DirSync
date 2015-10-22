using DirSync.Core.Domain;
using DirSync.Core.FileSystem.Watcher;
using DirSync.Core.Queue;

namespace DirSync.Core.FileSystem.Handler
{
    public interface IFileSystemEventHandler
    {
		FileSystemEventWatcherBase EventWatcher { get; }

		IConcurrentQueue<FileSystemErrorEventQueueItem> ProcessErrors { get; }

		void Process();
    }
}
