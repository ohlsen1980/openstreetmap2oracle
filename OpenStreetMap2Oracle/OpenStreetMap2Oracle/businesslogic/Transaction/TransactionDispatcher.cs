/*
 OpenStreetMap2Oracle - A windows application to export OpenStreetMap Data 
                (*.osm - files) to an oracle database
 -------------------------------------------------------------------------------
 Copyright (C) 2011  Christian Möller
 -------------------------------------------------------------------------------
 This program is free software; you can redistribute it and/or
 modify it under the terms of the GNU General Public License
 as published by the Free Software Foundation; either version 2
 of the License, or (at your option) any later version.

 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with this program; if not, write to the Free Software
 Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenStreetMap2Oracle.oracle;
using System.Data.OracleClient;
using System.Threading;
using OpenStreetMap2Oracle.controller;

namespace OpenStreetMap2Oracle.businesslogic.Transaction
{
    /// <summary>
    /// Helps dispatching a queue of OSMTransactionObjects
    /// </summary>
    public class TransactionDispatcher
    {
        private bool _mIsActive = false;

        private TransactionQueue _queue;
        /// <summary>
        /// Sets the transaction queue
        /// </summary>
        public TransactionQueue Queue
        {
            set { this._queue = value; }
        }

        /// <summary>
        /// Gets or sets the Dispatching Thread
        /// </summary>
        public Thread DispatcherThread
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a new instance of the transaction dispatcher
        /// </summary>
        public TransactionDispatcher()
        {
            this._queue = new TransactionQueue();
        }

        /// <summary>
        /// Creates a new instance of the transaction dispatcher
        /// </summary>
        /// <param name="queue">Transaction Queue </param>
        public TransactionDispatcher(TransactionQueue queue)
        {
            this._queue = queue;
        }

        /// <summary>
        /// Adds a new object to the queue
        /// </summary>
        /// <param name="obj"></param>
        public void Add(OSMTransactionObject obj)
        {
            _queue.Data.Add(obj);
        }

        /// <summary>
        /// Starts the dispatching Thread
        /// </summary>
        public void Start()
        {
            this._mIsActive = true;
            this.DispatcherThread = new Thread(new ThreadStart(dataDispatcher));
            this.DispatcherThread.Start();
        }

        /// <summary>
        /// Stops the dispatching Thread
        /// </summary>
        public void Stop()
        {
            this._mIsActive = false;
            this.DispatcherThread.Abort();
        }

        /// <summary>
        /// Dispatches the queue
        /// </summary>
        private void dataDispatcher()
        {
            try
            {
                while (this._mIsActive)
                {
                    if (this._queue.Data.Count > AppManagerController.DISPATCHER_FLUSH_THRESHOLD)
                    {
                        TransactionQueue tmpQueue = null;
                        lock (_queue)
                        {
                            tmpQueue = ((TransactionQueue)this._queue.Clone());
                            this._queue.Clear();
                        }
                        if (tmpQueue != null)
                        {
                            ProcessQueue(tmpQueue);
                        }
                    }
                    Thread.Sleep(100);
                }
            }
            catch (ThreadAbortException)
            {
            }
        }


        /// <summary>
        /// Processes the transaction queue.
        /// </summary>
        public void ProcessQueue(TransactionQueue queue)
        {
            lock (_queue)
            {
                Parallel.ForEach(this._queue.Data, osmObject =>
                {
                    DbExport dbHandle = OracleConnectionFactory.CreateConnection();
                    OracleCommand sql_cmd = dbHandle.DbConnection.CreateCommand();

                    sql_cmd.Transaction = dbHandle.Transaction;
                    sql_cmd.UpdatedRowSource = System.Data.UpdateRowSource.None;
                    dbHandle.execSqlCmd(osmObject.Query, sql_cmd);

                });

                OracleConnectionFactory.CommitAll();
            }
        }
    }
}
