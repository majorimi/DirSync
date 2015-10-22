using DirSync.Core.Queue.Factory.Context;

namespace DirSync.Core.Queue.Factory
{
    public class MemoryQueueFactory : IQueueFactory
    {
        public IConcurrentQueue<T> CreateConcurrentQueue<T>(QueueFactoryContext context)
        {
           return new MemoryConcurrenQueue<T>();
        }
    }
}
