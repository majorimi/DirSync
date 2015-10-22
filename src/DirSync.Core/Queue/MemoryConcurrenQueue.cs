using System.Collections.Concurrent;

namespace DirSync.Core.Queue
{
	public class MemoryConcurrenQueue<T> : IConcurrentQueue<T>
	{
		private readonly ConcurrentQueue<T> _memoryQueue = new ConcurrentQueue<T>();

		public int Count
		{
			get
			{
				return _memoryQueue.Count;
			}
		}

		public void Enqueue(T item)
		{
			_memoryQueue.Enqueue(item);
		}

		public bool TryDequeue(out T item)
		{
			return _memoryQueue.TryDequeue(out item);
		}
	}
}
