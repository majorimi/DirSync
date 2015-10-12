using DirSyncService.FileSystem.Watcher;

namespace DirSyncService.FileSystem.Handler
{
    public interface IFileSystemEventHandler
    {
        IFileSystemEventWatcher EventWatcher { get; }

        void Process();
    }
}
