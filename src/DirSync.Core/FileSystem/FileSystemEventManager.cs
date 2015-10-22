using System.Collections.Generic;
using DirSync.Core.FileSystem.Handler;

namespace DirSync.Core.FileSystem
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
