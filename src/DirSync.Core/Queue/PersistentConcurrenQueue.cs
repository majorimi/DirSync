﻿using System;
using System.Messaging;

namespace DirSync.Core.Queue
{
	public class PersistentConcurrenQueue<T> : IConcurrentQueue<T>
	{
	    private readonly string _queueName;
	    private readonly MessageQueue _messageQueue;
		private readonly TimeSpan _readTimeOut = TimeSpan.FromMilliseconds(150);

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
                var x = _messageQueue.GetMessageEnumerator2();
                int iCount = 0;
                while (x.MoveNext())
                {
                    iCount++;
                }
                return iCount;
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
		        var msg = _messageQueue.Receive(_readTimeOut);
		        msg.Formatter = new XmlMessageFormatter(new Type[] {typeof (T)});

				item = (T)msg.Body;
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
