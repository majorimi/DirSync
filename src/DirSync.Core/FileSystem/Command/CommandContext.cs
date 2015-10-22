namespace DirSync.Core.FileSystem.Command
{
    public abstract class CommandContext
    {
        public static CommandContext GetEmpty()
        {
            return new EmptyCommandContext();
        }
    }

    public class EmptyCommandContext : CommandContext
    { }   
}
