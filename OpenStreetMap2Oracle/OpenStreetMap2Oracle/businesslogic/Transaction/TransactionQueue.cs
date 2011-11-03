using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenStreetMap2Oracle.businesslogic.Transaction
{
    public class TransactionQueue
    {
        private List<OSMTransactionObject> _queue;

        public List<OSMTransactionObject> Data
        {
            get { return _queue; }
        }

        public TransactionQueue()
        {
            _queue = new List<OSMTransactionObject>();
        }

        public void Add(OSMTransactionObject obj)
        {
            lock (this._queue)
            {
                this._queue.Add(obj);
            }
        }

        public void AddRange(OSMTransactionObject[] objs)
        {
            lock (this._queue)
            {
                this._queue.AddRange(objs);
            }
        }

        public void Remove(OSMTransactionObject obj)
        {
            lock (this._queue)
            {
                if (this._queue.Contains(obj))
                {
                    this._queue.Remove(obj);
                }
            }
        }

        public void RemoveAt(int index)
        {
            lock (this._queue)
            {
                if (this._queue.Count > index)
                {
                    this._queue.RemoveAt(index);
                }
            }
        }

        public void Clear()
        {
            lock (this._queue)
            {
                this._queue.Clear();
            }
        }

    }
}
