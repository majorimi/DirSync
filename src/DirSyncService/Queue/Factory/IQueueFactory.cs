namespace DirSyncService.Queue.Factory
{
    public interface IQueueFactory
    {
        IConcurrentQueue<T> CreateConcurrentQueue<T>();
    }
}
