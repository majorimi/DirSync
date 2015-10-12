using System;
using System.Messaging;

namespace DirSyncService.Queue
{
	public class PersistentConcurrenQueue<T> : IConcurrentQueue<T>
	{
	    private readonly string _queueName;
	    private readonly MessageQueue _messageQueue;

        public PersistentConcurrenQueue(string queueName)
        {
            _queueName = queueName;

            if (!MessageQueue.Exists(_queueName))
                _messageQueue = MessageQueue.Create(_queueName);
            else
            {
                _messageQueue = new MessageQueue(_queueName);
            }
        }

		public int Count
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public void Enqueue(T item)
		{
            _messageQueue.Send(item);
        }

		public bool TryDequeue(out T item)
		{
		    try
		    {
		        var msg = _messageQueue.Receive(TimeSpan.FromMilliseconds(50));
		        msg.Formatter = new XmlMessageFormatter(new Type[] {typeof (T)});

		        item = (T)msg.Formatter.Read(msg);
                return true;
		    }
            catch (Exception)
            {
                item = default(T);
                return false;
            }
		}
	}
}
