namespace DirSyncService.FileSystem.Command
{
    public abstract class CommandResult
    {
        public CommandResult GetEmpty()
        {
            return new EmptyCommandResult();
        }
    }

    public class EmptyCommandResult : CommandResult
    { }   
}
