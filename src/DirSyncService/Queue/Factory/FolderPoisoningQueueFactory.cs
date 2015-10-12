namespace DirSyncService.Queue.Factory
{
    public class FolderPoisoningQueueFactory : IQueueFactory
    {
        public IConcurrentQueue<T> CreateConcurrentQueue<T>()
        {
            return new PersistentConcurrenQueue<T>("FolderPoisoning");
        }
    }
}
