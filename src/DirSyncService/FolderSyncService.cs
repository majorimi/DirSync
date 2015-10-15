using System.ServiceProcess;
using System.Threading;
using DirSyncService.Config;
using DirSyncService.FileSystem.Handler;
using DirSyncService.FileSystem;
using DirSyncService.Logging;
using DirSyncService.Installer;

namespace DirSyncService
{
	public partial class FolderSyncService : ServiceBase
	{
		private const int WaitTime = 10000;
		private bool _isRunnig = false;
		private readonly AutoResetEvent AutoReset = new AutoResetEvent(false);

		private FileSystemEventManager fileSystemEventManager;

	    public FolderSyncService()
	    {
	        log4net.Config.XmlConfigurator.Configure();

#if DEBUG
	        Logger.Current = log4net.LogManager.GetLogger("DirSyncDebug");
#else
	        Logger.Current = log4net.LogManager.GetLogger("DirSync");
#endif

            InitializeComponent();
	    }

	    public void Start()
		{
            Logger.Current.Info("Windows service is starting");

            OnStart(null);

            Logger.Current.Info("Windows service started");
        }

        protected override void OnStart(string[] args)
		{
			if (DirSyncConfiguration.SourceDir.Exists && DirSyncConfiguration.TargetDir.Exists)
			{
				RequestAdditionalTime(6000);

				//Check MSMQ
				WindwosServiceInstallerHelper.CheckMsmqService();

				fileSystemEventManager = FileSystemEventHandlerFactory.Init();


				Thread th = new Thread(Sync)
			    {
			        IsBackground = true,
			        Name = "FolderSyncService thread"
			    };

			    _isRunnig = true;
				th.Start();
			}
		}

		protected override void OnStop()
		{
            Logger.Current.Info("Windows service is stopping");

            _isRunnig = false;
			AutoReset.Set();

            Logger.Current.Info("Windows service stopped");
        }

        private void Sync()
		{
			while (_isRunnig)
			{
                Logger.Current.Debug("Start to process file system events");

                fileSystemEventManager.ProcessEvents();

                Logger.Current.Debug("File system events processed");

                AutoReset.WaitOne(WaitTime);
			}
		}
	}
}