namespace DirSync.Core.FileSystem.Command
{
    public abstract class CommandResult
    {
        public static CommandResult GetEmpty()
        {
            return new EmptyCommandResult();
        }
    }

    public class EmptyCommandResult : CommandResult
    { }   
}
