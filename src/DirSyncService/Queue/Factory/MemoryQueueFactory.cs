using DirSyncService.Queue.Factory.Context;

namespace DirSyncService.Queue.Factory
{
    public class MemoryQueueFactory : IQueueFactory
    {
        public IConcurrentQueue<T> CreateConcurrentQueue<T>(QueueFactoryContext context)
        {
           return new MemoryConcurrenQueue<T>();
        }
    }
}
