using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Permissions;
using System.ServiceProcess;

namespace DirSyncService.Installer
{
	[RunInstaller(true)]
    public partial class DirSyncInstaller : System.Configuration.Install.Installer
    {
		private const string ServiceName = "DirSync";
		private const string DisplayName = "DirSync Backup Service by Major";

        private ServiceInstaller serviceInstaller;
        private ServiceProcessInstaller processInstaller;

        public DirSyncInstaller()
        {
            // Instantiate installers for process and services.
            processInstaller = new ServiceProcessInstaller();
            serviceInstaller = new ServiceInstaller();

            // The services run under the system account.
            processInstaller.Account = ServiceAccount.LocalSystem;

            // The services are started Automatic.
            serviceInstaller.StartType = ServiceStartMode.Automatic;

			// ServiceName must equal those on ServiceBase derived classes.
			serviceInstaller.ServiceName = ServiceName;
			serviceInstaller.DisplayName = DisplayName;
			serviceInstaller.Description = "Real time Directory Synchronization Service for data backup";

            // Add installers to collection. Order is not important.
            Installers.Add(serviceInstaller);
            Installers.Add(processInstaller);
        }

        [SecurityPermission(SecurityAction.Demand)]
        protected override void OnAfterInstall(IDictionary savedState)
        {
            try
            {
                base.OnAfterInstall(savedState);

				//Interact with Desktop
				WindwosServiceInstallerHelper.SetWindowsService(ServiceName, "DesktopInteract", true);

				WindwosServiceInstallerHelper.StartWindowsService(ServiceName, false);

				WindwosServiceInstallerHelper.CheckMsmqService();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("DeploymentBoard IntegrationService - AfterInstall", ex.ToString(),
                    EventLogEntryType.Error);
            }
        }
    }
}