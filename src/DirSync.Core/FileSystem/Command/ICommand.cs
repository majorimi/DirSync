namespace DirSync.Core.FileSystem.Command
{
    public interface ICommand
    {
        TRet Execute<TRet, TContext>(TContext context) where TRet : CommandResult where TContext : CommandContext;
    }
}
