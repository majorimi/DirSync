using DirSyncService.Queue.Factory.Context;

namespace DirSyncService.Queue.Factory
{
    public interface IQueueFactory
    {
        IConcurrentQueue<T> CreateConcurrentQueue<T>(QueueFactoryContext context);
    }
}
