using System;
using DirSyncService.Queue;

namespace DirSyncService.FileSystem.Watcher
{
    public class FileWatcher : FileSystemWatcherBase, IFileSystemEventWatcher
    {
        public IConcurrentQueue<FileSystemEventQueueItem> Changes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void StartWatch(string path)
        {
            throw new NotImplementedException();
        }
    }
}
