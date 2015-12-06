using System;
using System.Collections.Generic;
using System.Threading;

namespace LighTake.MQS
{
    public class QueueMini : IQueueMini
    {
        private readonly Queue<object> _queue;

        private readonly string _name;

        private readonly object _dequeueLock=new object();

        private readonly ManualResetEvent _waitEnqueue = new ManualResetEvent(false);

        public string Name
        {
            get { return _name; }
        }

        public Func<IQueueMini, object, bool> BeforEnqueue { set; get; }

        public Action<IQueueMini, object> AfterEnqueue { get; set; }

        public Func<IQueueMini, bool> BeforDequeue { get; set; }

        public Action<IQueueMini, object> AfterDequeue { get; set; }

        public QueueMini(string name)
        {
            this._name = name;
            this._queue = new Queue<object>();
            this.AfterDequeue += AfterDequeueEvent;
            this.AfterEnqueue += AfterEnqueueEvent;
        }

        public int Length
        {
            get { return this._queue.Count; }
        }

        public void Enqueue(object value)
        {
            if (this.BeforEnqueue != null)
            {
                if (this.BeforEnqueue(this, value))
                {
                    _queue.Enqueue(value);

                    if (this.AfterEnqueue != null)
                        AfterEnqueue(this, value);
                }
                else
                {
                    throw new Exception("Cancelled");
                }
            }
            else
            {
                _queue.Enqueue(value);

                if (this.AfterEnqueue != null)
                    AfterEnqueue(this, value);
            }

        }

        public object Dequeue()
        {
            lock (_dequeueLock)
            {
                if (this.BeforDequeue != null)
                {
                    if (this.BeforDequeue(this))
                    {
                        object value = _queue.Dequeue();

                        if (this.AfterDequeue != null)
                            AfterDequeue(this, value);

                        return value;
                    }
                    else
                    {
                        throw new Exception("Cancelled");
                    }
                }
                else
                {
                    object value = _queue.Dequeue();

                    if (this.AfterDequeue != null)
                        AfterDequeue(this, value);

                    return value;
                }
            }
        }

        public object Peek()
        {
           return _queue.Peek();
        }

        public bool WaitEnqueue(int timeOut)
        {
            return _waitEnqueue.WaitOne(timeOut);
        }

        private void AfterDequeueEvent(IQueueMini queueMini, object value)
        {
            if (this.Length == 0)
            {
                _waitEnqueue.Reset();
            }
        }

        private void AfterEnqueueEvent(IQueueMini queueMini, object value)
        {
            _waitEnqueue.Set();
        }
    }
}