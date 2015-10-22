using System;
using DirSync.Core.FileSystem.Command;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DirSync.Core.Test
{
    [TestClass]
    public class CommandBaseTest
    {
        public class SimpleCommand : CommandBase
        {
            public override TRet Execute<TRet, TContext>(TContext context)
            {
                return default(TRet);
            }
        }

        public class RedoCommand : CommandBase, IRedoCommand
        {
            public override TRet Execute<TRet, TContext>(TContext context)
            {
                return default(TRet);
            }

            public TRet Redo<TRet, TContext>(TContext context) where TRet : CommandResult where TContext : CommandContext
            {
                return default(TRet);
            }
        }

        public class UndoCommand : CommandBase, IUndoCommand
        {
            public override TRet Execute<TRet, TContext>(TContext context)
            {
                return default(TRet);
            }

            public TRet Undo<TRet, TContext>(TContext context) where TRet : CommandResult where TContext : CommandContext
            {
                return default(TRet);
            }
        }

        [TestMethod]
        public void SimpleComamndTest()
        {
            var command = new SimpleCommand();

            Assert.IsTrue(command.CanExecute);

            Assert.IsFalse(command.CanRedo);
            Assert.IsFalse(command.CanUndo);
        }

        [TestMethod]
        public void RedoComamndTest()
        {
            var command = new RedoCommand();

            Assert.IsTrue(command.CanExecute);

            Assert.IsTrue(command.CanRedo);
            Assert.IsFalse(command.CanUndo);
        }

        [TestMethod]
        public void UndoComamndTest()
        {
            var command = new UndoCommand();

            Assert.IsTrue(command.CanExecute);

            Assert.IsFalse(command.CanRedo);
            Assert.IsTrue(command.CanUndo);
        }
    }
}