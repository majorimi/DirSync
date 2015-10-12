using System;
using System.IO;

namespace DirSyncService
{
    [Serializable()]
    public class FileSystemEventQueueItem
    {
        public FileSystemEventArgs EventArgs { get; private set; }

        public int TriedToProcess { get; protected set; }

        public FileSystemEventQueueItem(FileSystemEventArgs eventArgs)
        {
            EventArgs = eventArgs;
        }

        public int Tried()
        {
            return ++TriedToProcess;
        }
    }
}