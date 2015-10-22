using System.Collections.Generic;
using DirSync.Core.Config;
using DirSync.Core.Domain;
using DirSync.Core.Queue;
using DirSync.Core.Queue.Factory;
using DirSync.Core.Queue.Factory.Context;

namespace DirSync.Core.FileSystem.Handler
{
	public class FileSystemEventHandlerFactory
    {
		public static FileSystemEventManager Init()
		{
			var fileEventHandler = new FileEventHandler(InitQueue<FileSystemEventQueueItem>(new MemoryQueueFactory()), 
				InitQueue<FileSystemErrorEventQueueItem>(new MsmqFactory(), new MsmqFactoryContext("FilePoisoning")));

			var folderEventHandler = new FolderEventHandler(InitQueue<FileSystemEventQueueItem>(new MemoryQueueFactory()), 
				InitQueue<FileSystemErrorEventQueueItem>(new MsmqFactory(), new MsmqFactoryContext("FolderPoisoning")));

			folderEventHandler.EventWatcher.StartWatch(DirSyncConfiguration.SourceDir.FullName);
			fileEventHandler.EventWatcher.StartWatch(DirSyncConfiguration.SourceDir.FullName);
			return new FileSystemEventManager(new List<IFileSystemEventHandler>() { folderEventHandler, fileEventHandler });
		}

		private static IConcurrentQueue<T> InitQueue<T>(IQueueFactory queueFactory, QueueFactoryContext context = null)
		{
			if (context == null)
				context = new QueueFactoryContext();

			return queueFactory.CreateConcurrentQueue<T>(context);
		}
	}
}
