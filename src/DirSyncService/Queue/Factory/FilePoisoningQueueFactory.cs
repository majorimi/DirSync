namespace DirSyncService.Queue.Factory
{
    public class FilePoisoningQueueFactory : IQueueFactory
    {
        public IConcurrentQueue<T> CreateConcurrentQueue<T>()
        {
           return new PersistentConcurrenQueue<T>("FilePoisoning");
        }
    }
}
