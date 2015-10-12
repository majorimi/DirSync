using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirSyncService.Domain
{
	public class FileSystemRenameEvent : FileSystemChangeEvent
	{
		public string OldFullPath { get; set; }

		public string OldName { get; set; }

		/// <summary>
		/// Default constructor for serialize only!
		/// </summary>
		public FileSystemRenameEvent()
		{ }
	}
}
