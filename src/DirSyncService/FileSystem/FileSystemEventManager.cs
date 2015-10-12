using DirSyncService.FileSystem.Handler;
using System.Collections.Generic;

namespace DirSyncService.FileSystem
{
	public class FileSystemEventManager
	{
		private IEnumerable<IFileSystemEventHandler> _eventHandlers;

		public FileSystemEventManager(IEnumerable<IFileSystemEventHandler> eventHandlers)
		{
			_eventHandlers = eventHandlers;
		}

		public void ProcessEvents()
		{
			foreach (IFileSystemEventHandler eventHandler in _eventHandlers)
			{
				eventHandler.Process();
			}
		}
	}
}
