using DirSyncService.Domain;
using System;
using System.IO;

namespace DirSyncService
{
    [Serializable()]
    public class FileSystemEventQueueItem
    {
        public FileSystemChangeEvent EventArgs { get; set; }

        public int TriedToProcess { get;  set; }

		/// <summary>
		/// Default constructor for serialize only!
		/// </summary>
		public FileSystemEventQueueItem()
		{ }

        public FileSystemEventQueueItem(FileSystemEventArgs eventArgs)
        {
            EventArgs = FileSystemChangeEvent.Create(eventArgs);
        }

        public int Tried()
        {
            return ++TriedToProcess;
        }
    }
}