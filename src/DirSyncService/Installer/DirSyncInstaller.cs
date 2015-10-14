using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Security.Permissions;
using System.ServiceProcess;

namespace DirSyncService.Installer
{
    [RunInstaller(true)]
    public partial class DirSyncInstaller : System.Configuration.Install.Installer
    {
        private const string ServiceName = "Directory Sync Service by Major";
        private const string MsmqServiceName = "MSMQ";

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
                SetWindowsService(ServiceName, "DesktopInteract", true);

                StartWindowsService(ServiceName, false);

                CheckMSMQ();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("DeploymentBoard IntegrationService - AfterInstall", ex.ToString(),
                    EventLogEntryType.Error);
            }
        }

        private void CheckMSMQ()
        {
            List<ServiceController> services = ServiceController.GetServices().ToList();
            ServiceController msQue = services.Find(o => o.ServiceName == MsmqServiceName);
            if (msQue != null)
            {
                if (msQue.Status != ServiceControllerStatus.Running)
                {
                    StartWindowsService(MsmqServiceName, true);
                }
            }
            else
            {
                // Not installed? 
                /*
                http://blogs.msdn.com/b/johnbreakwell/archive/2007/06/19/minimalist-setup-script-for-msmq-unattended-installation.aspx
                https://technet.microsoft.com/en-us/library/cc731283(v=ws.10).aspx
                https://technet.microsoft.com/en-us/library/cc749102(v=ws.10).aspx
                */
            }
        }

        private void StartWindowsService(string serviceName, bool setToAutoStart)
        {
            using (var serviceController = new ServiceController(serviceName))
            {
                serviceController.Start();
            }

            if (setToAutoStart)
            {
                SetWindowsService(ServiceName, "StartMode", "Automatic");
            }
        }

        private void SetWindowsService(string serviceName, string parameterName, object parameterValue)
        {
            ConnectionOptions connectionOptions = new ConnectionOptions();
            connectionOptions.Impersonation = ImpersonationLevel.Impersonate;
            ManagementScope managementScope = new ManagementScope(@"root\CIMV2", connectionOptions);
            managementScope.Connect();

            using (ManagementObject wmiService = new ManagementObject("Win32_Service.Name='" + serviceName + "'"))
            {
                ManagementBaseObject InParam = wmiService.GetMethodParameters("Change");
                InParam[parameterName] = parameterValue;
                ManagementBaseObject managementObject = wmiService.InvokeMethod("Change", InParam, null);
            }
        }
    }
}