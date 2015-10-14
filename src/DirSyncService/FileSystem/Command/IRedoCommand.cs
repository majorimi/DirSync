namespace DirSyncService.FileSystem.Command
{
    public interface IRedoCommand
    {
        TRet Redo<TRet, TContext>(TContext context) where TRet : CommandResult where TContext : CommandContext;
    }
}
