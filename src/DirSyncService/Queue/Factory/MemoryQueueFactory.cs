namespace DirSyncService.Queue.Factory
{
    public class MemoryQueueFactory : IQueueFactory
    {
        public IConcurrentQueue<T> CreateConcurrentQueue<T>()
        {
           return new MemoryConcurrenQueue<T>();
        }
    }
}
