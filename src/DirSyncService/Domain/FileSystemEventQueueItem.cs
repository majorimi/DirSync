using System;
using System.IO;

namespace DirSyncService.Domain
{
    [Serializable()]
    public class FileSystemEventQueueItem
    {
        public FileSystemChangeEvent ChangeEvent { get; set; }

        public int TriedToProcess { get;  set; }

		/// <summary>
		/// Default constructor for serialize only!
		/// </summary>
		public FileSystemEventQueueItem()
		{ }

        public FileSystemEventQueueItem(FileSystemEventArgs eventArgs)
        {
            ChangeEvent = FileSystemChangeEvent.Create(eventArgs);
        }

        public int Tried()
        {
            return ++TriedToProcess;
        }
    }
}