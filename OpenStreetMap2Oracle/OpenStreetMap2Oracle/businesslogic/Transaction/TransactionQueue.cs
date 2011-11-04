using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenStreetMap2Oracle.businesslogic.Transaction
{
    /// <summary>
    /// A Queue holding OSMTransactionObjects
    /// </summary>
    public class TransactionQueue : ICollection<OSMTransactionObject>
    {
        private List<OSMTransactionObject> _queue;
        /// <summary>
        /// Gets the data currently attached to the list
        /// </summary>
        public List<OSMTransactionObject> Data
        {
            get { return _queue; }
        }

        /// <summary>
        /// Creates a new instance of the Transaction Queue
        /// </summary>
        public TransactionQueue()
        {
            _queue = new List<OSMTransactionObject>();
        }

        /// <summary>
        /// Adds a new object to the queue
        /// </summary>
        /// <param name="obj">New Transaction object</param>
        public void Add(OSMTransactionObject obj)
        {
            lock (this._queue)
            {
                this._queue.Add(obj);
            }
        }

        /// <summary>
        /// Adds multiple objects to the queue
        /// </summary>
        /// <param name="objs">Array of transaction objects</param>
        public void AddRange(OSMTransactionObject[] objs)
        {
            lock (this._queue)
            {
                this._queue.AddRange(objs);
            }
        }

        /// <summary>
        /// Removes the passed osm object from the queue
        /// </summary>
        /// <param name="obj">object to delete from queue</param>
        public bool Remove(OSMTransactionObject obj)
        {
            lock (this._queue)
            {
                if (this._queue.Contains(obj))
                {
                    this._queue.Remove(obj);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Removes the item at the defined index
        /// </summary>
        /// <param name="index">index to remove</param>
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

        /// <summary>
        /// Clears the queue.
        /// </summary>
        public void Clear()
        {
            lock (this._queue)
            {
                this._queue.Clear();
            }
        }

        /// <summary>
        /// Returns true if the item is present in the queue
        /// </summary>
        /// <param name="item">The item to search for</param>
        /// <returns></returns>
        public bool Contains(OSMTransactionObject item)
        {
            return (this._queue.Contains(item));
        }

        /// <summary>
        /// Stub!
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(OSMTransactionObject[] array, int arrayIndex)
        {
            
        }

        /// <summary>
        /// Returns the length of the queue
        /// </summary>
        public int Count
        {
            get { return Data.Count; }
        }

        /// <summary>
        /// Returns a value indicating the queue is read-only
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Returns a new enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<OSMTransactionObject> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a new enumerator
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }


    }
}
