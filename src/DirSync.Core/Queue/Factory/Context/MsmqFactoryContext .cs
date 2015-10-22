namespace DirSync.Core.Queue.Factory.Context
{
	public class MsmqFactoryContext : QueueFactoryContext
	{
		public string QueueName { get; private set; }

		public MsmqFactoryContext(string queueName)
		{
			QueueName = queueName;
		}
	}
}
