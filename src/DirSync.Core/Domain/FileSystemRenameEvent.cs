namespace DirSync.Core.Domain
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
