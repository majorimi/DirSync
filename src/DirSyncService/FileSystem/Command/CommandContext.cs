namespace DirSyncService.FileSystem.Command
{
    public abstract class CommandContext
    {
        public CommandContext GetEmpty()
        {
            return new EmptyCommandContext();
        }
    }

    public class EmptyCommandContext : CommandContext
    { }   
}
