namespace DirSync.Core.FileSystem.Command.File
{
    public class FileCopyCommand : CommandBase, IRedoCommand
    {
        public override TRet Execute<TRet, TContext>(TContext context)
        {


            return (TRet)CommandResult.GetEmpty();
        }

        TRet IRedoCommand.Redo<TRet, TContext>(TContext context)
        {
            return Execute<TRet, EmptyCommandContext>(null);
        }
    }
}
