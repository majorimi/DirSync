using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Management;
using System.Security.Permissions;
using System.ServiceProcess;

namespace DirSyncService.Installer
{
    [RunInstaller(true)]
    public partial class DirSyncInstaller : System.Configuration.Install.Installer
    {
        private const string ServiceName = "Directory Sync Service by Major";

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
            serviceInstaller.Description = "Directory Sync Service please check the config file to set the Soruce and Target folders";

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
                ConnectionOptions connectionOptions = new ConnectionOptions();
                connectionOptions.Impersonation = ImpersonationLevel.Impersonate;
                ManagementScope managementScope = new ManagementScope(@"root\CIMV2", connectionOptions);
                managementScope.Connect();

                using (ManagementObject wmiService = new ManagementObject("Win32_Service.Name='" + ServiceName + "'"))
                {
                    ManagementBaseObject InParam = wmiService.GetMethodParameters("Change");
                    InParam["DesktopInteract"] = true;
                    ManagementBaseObject managementObject = wmiService.InvokeMethod("Change", InParam, null);
                }

                //Start service
                ServiceController serviceController = new ServiceController(ServiceName);
                serviceController.Start();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("DeploymentBoard IntegrationService - AfterInstall", ex.ToString(), EventLogEntryType.Error);
            }
        }
    }
}