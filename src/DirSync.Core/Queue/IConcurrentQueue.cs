﻿namespace DirSync.Core.Queue
{
	public interface IConcurrentQueue<T>
	{
		int Count { get; }

		void Enqueue(T item);

		bool TryDequeue(out T item);
    }
}
