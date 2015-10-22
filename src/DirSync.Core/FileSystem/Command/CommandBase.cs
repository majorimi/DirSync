namespace DirSync.Core.FileSystem.Command
{
    public abstract class CommandBase : ICommand
    {
        public virtual bool CanExecute
        {
            get { return true; }
        }

        public virtual bool CanRedo
        {
            get { return this is IRedoCommand; }
        }

        public virtual bool CanUndo
        {
            get { return this is IUndoCommand; }
        }

        public abstract TRet Execute<TRet, TContext>(TContext context) where TRet : CommandResult where TContext : CommandContext;
    }
}
