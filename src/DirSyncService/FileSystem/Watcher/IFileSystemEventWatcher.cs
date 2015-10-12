using DirSyncService.Queue;

namespace DirSyncService.FileSystem.Watcher
{
    public interface IFileSystemEventWatcher
    {
        IConcurrentQueue<FileSystemEventQueueItem> Changes { get; } 

        void StartWatch(string path);
    }
}
