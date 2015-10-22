using System;
using System.IO;
using System.Threading;
using DirSync.Core.Config;
using DirSync.Core.Domain;
using DirSync.Core.FileSystem.Watcher;
using DirSync.Core.Logging;
using DirSync.Core.Queue;

namespace DirSync.Core.FileSystem.Handler
{
	public class FileEventHandler : IFileSystemEventHandler
	{
		public FileSystemEventWatcherBase EventWatcher
		{
			get; private set;
		}

		public IConcurrentQueue<FileSystemErrorEventQueueItem> ProcessErrors
		{
			get; private set;
		}

		public FileEventHandler(IConcurrentQueue<FileSystemEventQueueItem> eventQueue, IConcurrentQueue<FileSystemErrorEventQueueItem> poisonQueue)
		{
			EventWatcher = new FileEventWatcher(eventQueue);
			ProcessErrors = poisonQueue;
        }

		public void Process()
		{
			FileSystemEventQueueItem queueItem = null;
			while (EventWatcher.Changes.TryDequeue(out queueItem))
			{
				try
				{
					HandleFileChanges(queueItem);
				}
				catch (Exception ex)
				{
					if (queueItem.Tried() <= DirSyncConfiguration.MaxTrying)
					{
						EventWatcher.Changes.Enqueue(queueItem);
						Thread.Sleep(200);
					}
					else
					{
						ProcessErrors.Enqueue(new FileSystemErrorEventQueueItem(queueItem, ex));
						Logger.Current.Error($"{this.GetType().Name} failed to sync file system event. Handler reached the maximum number of retrying event was sent to the poison queue. Exception: {ex.ToString()}");
					}
				}
			}
		}

		private void HandleFileChanges(FileSystemEventQueueItem queueItem)
		{
			var filePath = MapSourceFileToTargetFilePath(queueItem.ChangeEvent.FullPath, queueItem.ChangeEvent.ChangeType);

			switch (queueItem.ChangeEvent.ChangeType)
			{
				case WatcherChangeTypes.Created:
					{
						if (!File.Exists(filePath))
						{
							using (File.Create(filePath))
							{ }
						}
						break;
					}
				case WatcherChangeTypes.Deleted:
					{
						if (File.Exists(filePath))
						{
							if (DirSyncConfiguration.IsBackUpMode)
							{
								FileInfo fi = new FileInfo(filePath);
								var backUpPath = Path.Combine(fi.DirectoryName, DirSyncConfiguration.BackUpDirName);
								if (!Directory.Exists(backUpPath))
									Directory.CreateDirectory(backUpPath);

								File.Move(filePath, Path.Combine(backUpPath, fi.Name));
							}
							else
							{
								File.SetAttributes(filePath, FileAttributes.Normal);
								File.Delete(filePath);
							}
						}
						break;
					}
				case WatcherChangeTypes.Changed:
					File.Copy(queueItem.ChangeEvent.FullPath, filePath, true);
					break;
				case WatcherChangeTypes.Renamed:
					{
						var renamedEventArgs = (FileSystemRenameEvent)queueItem.ChangeEvent;
						var mappedPath = MapSourceFileToTargetFilePath(renamedEventArgs.OldFullPath, queueItem.ChangeEvent.ChangeType);
						if (File.Exists(mappedPath) && !File.Exists(filePath))
						{
							File.Move(mappedPath, filePath);
						}
						break;
					}
					//case WatcherChangeTypes.All:
					//	break;
					//default:
					//	throw new ArgumentOutOfRangeException();
			}
		}

		private string MapSourceFileToTargetFilePath(string sourceFileFullPath, WatcherChangeTypes changeType)
		{
			FileInfo fi = new FileInfo(sourceFileFullPath);
			string internalPath = fi.FullName.Replace(DirSyncConfiguration.SourceDir.FullName, string.Empty).Replace(fi.Name, string.Empty);

			string fullDir = Path.Combine(DirSyncConfiguration.TargetDir.FullName, internalPath);
			if (changeType != WatcherChangeTypes.Deleted &&  !Directory.Exists(fullDir))
				Directory.CreateDirectory(fullDir);

			return Path.Combine(fullDir, fi.Name);
		}
	}
}
