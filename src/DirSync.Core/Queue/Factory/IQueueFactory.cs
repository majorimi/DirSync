using DirSync.Core.Queue.Factory.Context;

namespace DirSync.Core.Queue.Factory
{
    public interface IQueueFactory
    {
        IConcurrentQueue<T> CreateConcurrentQueue<T>(QueueFactoryContext context);
    }
}
