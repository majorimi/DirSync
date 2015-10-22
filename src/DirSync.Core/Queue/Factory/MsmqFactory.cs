using System;
using DirSync.Core.Queue.Factory.Context;

namespace DirSync.Core.Queue.Factory
{
    public class MsmqFactory : IQueueFactory
    {
		public IConcurrentQueue<T> CreateConcurrentQueue<T>(QueueFactoryContext context)
		{
			var c = context as MsmqFactoryContext;
			if (c == null)
				throw new ArgumentException($"{typeof(MsmqFactory)} require an instance of an object of: {typeof(MsmqFactoryContext)}");

			return new PersistentConcurrenQueue<T>($".\\PRIVATE$\\{c.QueueName}");
		}
    }
}
