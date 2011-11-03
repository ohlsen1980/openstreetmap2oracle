using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenStreetMap2Oracle.oracle;
using System.Data.OracleClient;

namespace OpenStreetMap2Oracle.businesslogic.Transaction
{
    public class TransactionDispatcher
    {
        private TransactionQueue _queue;
        public TransactionQueue Queue
        {
            set { this._queue = value; }
        }

        public TransactionDispatcher()
        {
        }

        public TransactionDispatcher(TransactionQueue queue)
        {
            this._queue = queue;
        }



        public void ProcessQueue() 
        {
            lock (_queue)
            {
                Parallel.ForEach(this._queue.Data, osmObject =>
                {
                    DbExport dbHandle = OracleConnectionFactory.CreateConnection();

                    using (OracleCommand sql_cmd = dbHandle.DbConnection.CreateCommand())
                    {
                        sql_cmd.Transaction = dbHandle.Transaction;
                        sql_cmd.UpdatedRowSource = System.Data.UpdateRowSource.None;
                        dbHandle.execSqlCmd(osmObject.Query, sql_cmd);
                        dbHandle.Transaction.Commit();
                        //dbHandle.DbConnection.Close();

                        OracleConnectionFactory.FreeConnection(dbHandle);
                    }
                });
            }
        }
    }
}
