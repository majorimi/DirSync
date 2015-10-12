using System;
using DirSyncService.FileSystem.Watcher;
using DirSyncService.Queue;
using System.Threading;
using System.IO;
using DirSyncService.Config;
using DirSyncService.Domain;

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
						//log
					}
				}
			}
		}

		private void HandleFolderChanges(FileSystemEventQueueItem queueItem)
		{
			var folderPath = MapSourceDirToTargetDirPath(queueItem.EventArgs.FullPath);

			switch (queueItem.EventArgs.ChangeType)
			{
				case WatcherChangeTypes.Created:
					{
						if (!Directory.Exists(folderPath))
						{
							Directory.CreateDirectory(MapSourceDirToTargetDirPath(queueItem.EventArgs.FullPath));
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

								Directory.Move(folderPath, Path.Combine(backUpPath, queueItem.EventArgs.Name));
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
						var renamedEventArgs = (FileSystemRenameEvent)queueItem.EventArgs;
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
