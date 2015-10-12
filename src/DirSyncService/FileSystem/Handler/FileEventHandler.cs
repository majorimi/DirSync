using System;
using DirSyncService.FileSystem.Watcher;
using DirSyncService.Queue;
using System.Threading;
using System.IO;
using DirSyncService.Config;
using DirSyncService.Domain;

namespace DirSyncService.FileSystem.Handler
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
						//log
					}
				}
			}
		}

		private void HandleFileChanges(FileSystemEventQueueItem queueItem)
		{
			var filePath = MapSourceFileToTargetFilePath(queueItem.EventArgs.FullPath);

			switch (queueItem.EventArgs.ChangeType)
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
					File.Copy(queueItem.EventArgs.FullPath, filePath, true);
					break;
				case WatcherChangeTypes.Renamed:
					{
						var renamedEventArgs = (FileSystemRenameEvent)queueItem.EventArgs;
						var mappedPath = MapSourceFileToTargetFilePath(renamedEventArgs.OldFullPath);
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

		private string MapSourceFileToTargetFilePath(string sourceFileFullPath)
		{
			FileInfo fi = new FileInfo(sourceFileFullPath);
			string internalPath = fi.FullName.Replace(DirSyncConfiguration.SourceDir.FullName, string.Empty).Replace(fi.Name, string.Empty);

			string fullDir = Path.Combine(DirSyncConfiguration.TargetDir.FullName, internalPath);
			if (!Directory.Exists(fullDir))
				Directory.CreateDirectory(fullDir);

			return Path.Combine(fullDir, fi.Name);
		}
	}
}
