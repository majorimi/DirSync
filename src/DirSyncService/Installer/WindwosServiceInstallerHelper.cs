using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Permissions;
using System.ServiceProcess;

namespace DirSyncService.Installer
{
	public static class WindwosServiceInstallerHelper
	{
		private const string MsmqServiceName = "MSMQ";

		[SecurityPermission(SecurityAction.Demand)]
		public static void CheckMsmqService()
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
				using (Process p = new Process())
				{
					ProcessStartInfo start = new ProcessStartInfo(Path.Combine(Environment.GetEnvironmentVariable("windir"), "sysnative\\Dism.exe"),
						"/Online /NoRestart /English /Enable-Feature /FeatureName:MSMQ-Container /FeatureName:MSMQ-Server");

					start.UseShellExecute = false;
					p.StartInfo = start;

					p.Start();
					p.WaitForExit();
				}
			}
		}

		[SecurityPermission(SecurityAction.Demand)]
		public static void StartWindowsService(string serviceName, bool setToAutoStart)
		{
			if (setToAutoStart)
			{
				SetWindowsService(serviceName, "StartMode", "Automatic");
			}

			using (var serviceController = new ServiceController(serviceName))
			{
				serviceController.Start();
				serviceController.WaitForStatus(ServiceControllerStatus.Running);
			}
		}

		[SecurityPermission(SecurityAction.Demand)]
		public static void SetWindowsService(string serviceName, string parameterName, object parameterValue)
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
