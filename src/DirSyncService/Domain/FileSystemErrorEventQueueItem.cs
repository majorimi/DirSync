using System;

namespace DirSyncService
{
    [Serializable()]
    public class FileSystemErrorEventQueueItem : FileSystemEventQueueItem
    {
        public DateTime LastProcessTime { get; private set; }

        public Exception ProcessException { get; private set; }

        public FileSystemErrorEventQueueItem(FileSystemEventQueueItem eventQueueItem, Exception ex)
            : base(eventQueueItem.EventArgs)
        {
            base.TriedToProcess = eventQueueItem.TriedToProcess;
            ProcessException = ex;
            LastProcessTime = DateTime.Now;
        }
    }
}