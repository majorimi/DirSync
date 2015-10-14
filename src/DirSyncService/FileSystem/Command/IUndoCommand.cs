namespace DirSyncService.FileSystem.Command
{
    public interface IUndoCommand
    {
        TRet Undo<TRet, TContext>(TContext context) where TRet : CommandResult where TContext : CommandContext;
    }
}
