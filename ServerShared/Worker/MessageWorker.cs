using ServerShared.Model;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ServerShared.Worker
{
    public class Message
    {
        public Session Session { get; init; }

        public Protocols.Id.Request Id { get; init; }

        public byte[] Data { get; init; }

        public Action Action { get; set; }
    }

    public class MessageWorker
    {
        private Thread GlobalThread { get; set; }

        public delegate bool Callback(Message message);

        public Callback MessageCallback { get; set; }

        private ConcurrentQueue<Message> GlobalQueue { get; } = new();

        public MessageWorker()
        {
            GlobalThread = new Thread(GlobalRun);
        }

        public void Push(Message message)
        {
            GlobalQueue.Enqueue(message);
        }

        public void Start()
        {
            GlobalThread.Start();
        }

        public void Join()
        {
            GlobalThread.Join();
        }

        private void GlobalRun()
        {
            while (true)
            {
                if (GlobalQueue.TryDequeue(out var message))
                {
                    MessageCallback?.Invoke(message);
                }
            }
        }
    }
}
