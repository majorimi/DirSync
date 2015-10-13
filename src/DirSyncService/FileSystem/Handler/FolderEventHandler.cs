using System;
using DirSyncService.FileSystem.Watcher;
using DirSyncService.Queue;
using System.Threading;
using System.IO;
using DirSyncService.Config;
using DirSyncService.Domain;
using DirSyncService.Logging;

namespace DirSyncService.FileSystem.Handler
{
	public class FolderEventHandler : IFileSystemEventHandler
	{
		public FileSystemEventWatcherBase EventWatcher
		{
			get; private set;
		}

		public IConcurrentQueue<FileSystemErrorEventQueueItem> ProcessErrors
		{
			get; private set;
		}

		public FolderEventHandler(IConcurrentQueue<FileSystemEventQueueItem> eventQueue, IConcurrentQueue<FileSystemErrorEventQueueItem> poisonQueue)
		{
			EventWatcher = new FolderEventWatcher(eventQueue);
			ProcessErrors = poisonQueue;
        }

		public void Process()
		{
			FileSystemEventQueueItem queueItem = null;
			while (EventWatcher.Changes.TryDequeue(out queueItem))
			{
				try
				{
					HandleFolderChanges(queueItem);
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

		private void HandleFolderChanges(FileSystemEventQueueItem queueItem)
		{
			var folderPath = MapSourceDirToTargetDirPath(queueItem.ChangeEvent.FullPath);

			switch (queueItem.ChangeEvent.ChangeType)
			{
				case WatcherChangeTypes.Created:
					{
						if (!Directory.Exists(folderPath))
						{
							Directory.CreateDirectory(MapSourceDirToTargetDirPath(queueItem.ChangeEvent.FullPath));
						}
						break;
					}
				case WatcherChangeTypes.Deleted:
					{
						DirectoryInfo di = new DirectoryInfo(folderPath);
						if (di.Exists)
						{
							if (DirSyncConfiguration.IsBackUpMode)
							{
								var backUpPath = Path.Combine(di.Parent.FullName, DirSyncConfiguration.BackUpDirName);
								if (!Directory.Exists(backUpPath))
									Directory.CreateDirectory(backUpPath);

								di.MoveTo(Path.Combine(backUpPath, queueItem.ChangeEvent.Name));
							}
							else
							{
								DeleteFileSystemInfo(di);
							}
						}
						break;
					}
				case WatcherChangeTypes.Renamed:
					{
						var renamedEventArgs = (FileSystemRenameEvent)queueItem.ChangeEvent;
						var mappedPath = MapSourceDirToTargetDirPath(renamedEventArgs.OldFullPath);
						if (Directory.Exists(mappedPath) && !Directory.Exists(folderPath))
						{
							Directory.Move(mappedPath, folderPath);
						}
						break;
					}
					//case WatcherChangeTypes.Changed:
					//  break;						
					//case WatcherChangeTypes.All:
					//	break;
					//default:
					//	throw new ArgumentOutOfRangeException();
			}
		}

		private void DeleteFileSystemInfo(FileSystemInfo fileSystemInfo)
		{
			var directoryInfo = fileSystemInfo as DirectoryInfo;
			if (directoryInfo != null)
			{
				foreach (var childInfo in directoryInfo.GetFileSystemInfos())
				{
					DeleteFileSystemInfo(childInfo);
				}
			}

			fileSystemInfo.Attributes = FileAttributes.Normal;
			fileSystemInfo.Delete();
		}

		private string MapSourceDirToTargetDirPath(string sourceDirFullPath)
		{
			DirectoryInfo di = new DirectoryInfo(sourceDirFullPath);
			string internalPath = di.FullName.Replace(DirSyncConfiguration.SourceDir.FullName, string.Empty);

			return Path.Combine(DirSyncConfiguration.TargetDir.FullName, internalPath);
		}
	}
}
