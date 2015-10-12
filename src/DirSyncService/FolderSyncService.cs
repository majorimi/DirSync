using System.ServiceProcess;
using System.Threading;
using DirSyncService.Config;
using DirSyncService.FileSystem.Handler;
using DirSyncService.FileSystem;

namespace DirSyncService
{
	public partial class FolderSyncService : ServiceBase
	{
		private const int WaitTime = 10000;
		private bool _isRunnig = false;
		private readonly AutoResetEvent AutoReset = new AutoResetEvent(false);
		private readonly FileSystemEventManager fileSystemEventManager;

		public FolderSyncService()
		{
			InitializeComponent();
			fileSystemEventManager = FileSystemEventHandlerFactory.Init();
        }

		public void Start()
		{
			OnStart(null);
		}

		protected override void OnStart(string[] args)
		{
			if (DirSyncConfiguration.SourceDir.Exists && DirSyncConfiguration.TargetDir.Exists)
			{
				Thread th = new Thread(Sync);
				th.IsBackground = true;
				th.Name = "FolderSyncService thread";

				_isRunnig = true;
				th.Start();
			}
		}

		protected override void OnStop()
		{
			_isRunnig = false;
			AutoReset.Set();
		}

		private void Sync()
		{
			while (_isRunnig)
			{
				fileSystemEventManager.ProcessEvents();

				AutoReset.WaitOne(WaitTime);
			}
		}
	}
}