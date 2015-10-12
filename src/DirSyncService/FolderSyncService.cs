using System;
using System.Configuration;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using DirSyncService.Queue;
using DirSyncService.Queue.Factory;

namespace DirSyncService
{
    public partial class FolderSyncService : ServiceBase
    {
        #region Private members

        private const int WaitTime = 10000;
        private const int MaxTrying = 10;
        private bool _isRunnig = false;

        private readonly AutoResetEvent AutoReset = new AutoResetEvent(false);

        private readonly IConcurrentQueue<FileSystemEventQueueItem> _fileEvents;
        private readonly IConcurrentQueue<FileSystemEventQueueItem> _folderEvents;
		private readonly IConcurrentQueue<FileSystemErrorEventQueueItem> _filePoisoningEvents;
		private readonly IConcurrentQueue<FileSystemErrorEventQueueItem> _folderPoisoningEvents;

		private readonly DirectoryInfo _sourceDir;
        private readonly DirectoryInfo _targetDir;

        #endregion

        public bool IsBackUpMode
		{
			get
			{
				return Convert.ToBoolean(ConfigurationManager.AppSettings["BackUpMode"]);
            }
		}

		public string BackUpDirName
		{
			get
			{
				return ConfigurationManager.AppSettings["BackUpDirName"];
			}
		}

		public FolderSyncService()
        {
            InitializeComponent();

            _sourceDir = new DirectoryInfo(ConfigurationManager.AppSettings["SourceDir"]);
            _targetDir = new DirectoryInfo(ConfigurationManager.AppSettings["TargetDir"]);

            MemoryQueueFactory memoryConcurrentQueueFactory = new MemoryQueueFactory();
		    _fileEvents = InitQueue<FileSystemEventQueueItem>(memoryConcurrentQueueFactory);
            _folderEvents = InitQueue<FileSystemEventQueueItem>(memoryConcurrentQueueFactory);

		    _filePoisoningEvents = InitQueue<FileSystemErrorEventQueueItem>(new FilePoisoningQueueFactory());
            _folderPoisoningEvents = InitQueue<FileSystemErrorEventQueueItem>(new FilePoisoningQueueFactory());
        }

        private IConcurrentQueue<T> InitQueue<T>(IQueueFactory queueFactory)
        {
            return queueFactory.CreateConcurrentQueue<T>();
        }

        //public void Start()
        //{
        //    OnStart(null);
        //}

        protected override void OnStart(string[] args)
        {
            if (_sourceDir.Exists && _targetDir.Exists)
            {
                Thread th = new Thread(Sync);
                th.IsBackground = true;
                th.Name = "FolderSyncService thread";
                th.Start();
                _isRunnig = true;
            }
        }

        protected override void OnStop()
        {
            _isRunnig = false;
            AutoReset.Set();
        }


        private void Sync()
        {
            CreateFileWatcher(FileEvents, FileEvents, NotifyFilters.FileName | NotifyFilters.Size);
            CreateFileWatcher(FolderEvents, FolderEvents, NotifyFilters.DirectoryName);

            while (_isRunnig)
            {
                HandleFolderChanges();
                HandleFileChanges();

                AutoReset.WaitOne(WaitTime);
            }
        }

		private void HandleFileChanges()
		{
			FileSystemEventQueueItem queueItem = null;
			while (_fileEvents.TryDequeue(out queueItem))
			{
				try
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
									if (IsBackUpMode)
									{
										FileInfo fi = new FileInfo(filePath);
									    var backUpPath = Path.Combine(fi.DirectoryName, BackUpDirName);
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
								RenamedEventArgs renamedEventArgs = (RenamedEventArgs)queueItem.EventArgs;
								File.Move(MapSourceFileToTargetFilePath(renamedEventArgs.OldFullPath), filePath);
								break;
							}
						//case WatcherChangeTypes.All:
						//	break;
						//default:
						//	throw new ArgumentOutOfRangeException();
					}
				}
				catch (Exception ex)
				{
					if (queueItem.Tried() <= MaxTrying)
					{
						_fileEvents.Enqueue(queueItem);
						Thread.Sleep(200);
					}
					else
					{
						_filePoisoningEvents.Enqueue(new FileSystemErrorEventQueueItem(queueItem, ex));
						//log
					}
				}
			}
		}

        private void HandleFolderChanges()
        {
            FileSystemEventQueueItem queueItem = null;
            while (_folderEvents.TryDequeue(out queueItem))
            {
                try
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
                                if (IsBackUpMode)
                                {
                                    var backUpPath = Path.Combine(folderPath, BackUpDirName);
                                    if (!Directory.Exists(backUpPath))
                                        Directory.CreateDirectory(backUpPath);

                                    Directory.Move(folderPath, backUpPath);
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
                            RenamedEventArgs renamedEventArgs = (RenamedEventArgs) queueItem.EventArgs;
                            Directory.Move(MapSourceDirToTargetDirPath(renamedEventArgs.OldFullPath), folderPath);
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
                catch (Exception ex)
                {
                    if (queueItem.Tried() <= MaxTrying)
                    {
                        _folderEvents.Enqueue(queueItem);
                        Thread.Sleep(200);
                    }
                    else
                    {
                        _folderPoisoningEvents.Enqueue(new FileSystemErrorEventQueueItem(queueItem, ex));
                        //log
                    }
                }
            }
        }


        private void CreateFileWatcher(FileSystemEventHandler eventHandler, RenamedEventHandler renamedEventHandler, NotifyFilters filters)
        {
            FileSystemWatcher fsw = new FileSystemWatcher(_sourceDir.FullName);
            fsw.IncludeSubdirectories = true;
            fsw.EnableRaisingEvents = true;
            fsw.NotifyFilter = filters;
            fsw.InternalBufferSize = 20000; 

            fsw.Changed += eventHandler;
            fsw.Created += eventHandler;
            fsw.Deleted += eventHandler;
            fsw.Renamed += renamedEventHandler;

            fsw.Error += FileSystemWatcherError;
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
            string internalPath = di.FullName.Replace(_sourceDir.FullName, string.Empty);

            return Path.Combine(_targetDir.FullName, internalPath);
        }

        private string MapSourceFileToTargetFilePath(string sourceFileFullPath)
        {
            FileInfo fi = new FileInfo(sourceFileFullPath);
            string internalPath = fi.FullName.Replace(_sourceDir.FullName, string.Empty).Replace(fi.Name, string.Empty);

            string fullDir = Path.Combine(_targetDir.FullName, internalPath);
            if (!Directory.Exists(fullDir))
                Directory.CreateDirectory(fullDir);

            return Path.Combine(fullDir, fi.Name);
        }

        private void FileEvents(object sender, FileSystemEventArgs e)
        {
            _fileEvents.Enqueue(new FileSystemEventQueueItem(e));
        }

        private void FolderEvents(object sender, FileSystemEventArgs e)
        {
            _folderEvents.Enqueue(new FileSystemEventQueueItem(e));
        }

        private void FileSystemWatcherError(object sender, ErrorEventArgs e)
        {
            //log...
        }
    }
}