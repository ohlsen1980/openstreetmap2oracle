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

namespace OpenStreetMap2Oracle.businesslogic.Transaction
{
    /// <summary>
    /// Helps dispatching a queue of OSMTransactionObjects
    /// </summary>
    public class TransactionDispatcher
    {
        private TransactionQueue _queue;
        /// <summary>
        /// Sets the transaction queue
        /// </summary>
        public TransactionQueue Queue
        {
            set { this._queue = value; }
        }

        /// <summary>
        /// Creates a new instance of the transaction dispatcher
        /// </summary>
        public TransactionDispatcher()
        {
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
        /// Processes the transaction queue.
        /// </summary>
        public void ProcessQueue()
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
