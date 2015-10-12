using System;

namespace DirSyncService
{
    [Serializable()]
    public class FileSystemErrorEventQueueItem : FileSystemEventQueueItem
    {
        public DateTime LastProcessTime { get; set; }

        public string ProcessException { get; set; }

		/// <summary>
		/// Default constructor for serialize only!
		/// </summary>
		public FileSystemErrorEventQueueItem()
		{}

        public FileSystemErrorEventQueueItem(FileSystemEventQueueItem eventQueueItem, Exception ex)
            : base()
        {
			base.EventArgs = eventQueueItem.EventArgs;
			base.TriedToProcess = eventQueueItem.TriedToProcess;

			TriedToProcess = eventQueueItem.TriedToProcess;
            ProcessException = ex.ToString();
            LastProcessTime = DateTime.Now;
        }
    }
}