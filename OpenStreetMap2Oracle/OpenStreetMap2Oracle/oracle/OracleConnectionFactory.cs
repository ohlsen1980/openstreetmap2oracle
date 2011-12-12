using System;
using System.Data.OracleClient;
using System.Collections.Generic;
using System.Threading;

namespace OpenStreetMap2Oracle.oracle
{
    /// <summary>
    /// Class to handle Oracle Connections
    /// Singleton Implementation
    /// </summary>
    public class OracleConnectionFactory
    {
        private static OpenStreetMap2Oracle.oracle.OracleConnectionFactory _instance = null;
        private List<DbExport> _connectionList = new List<DbExport>();
        int numberConnections = 20;
        private UserCredentials _credentials = null;

        public UserCredentials Credentials
        {
            get { return _credentials; }
            set { _credentials = value; }
        }

        private DbExport FreeConnection
        {
            get
            {
                DbExport retVal = null;
                do
                {
                    foreach (DbExport conn in _connectionList)
                    {
                        if (conn.DbConnection.State == System.Data.ConnectionState.Closed)
                            retVal = conn;
                    }

                }
                while(retVal == null);
                OpenConnection(retVal);
                return retVal;
            }
        }

        public void OpenConnection(DbExport conn)
        {
            if (conn.DbConnection.State == System.Data.ConnectionState.Closed)
                conn.DbConnection.Open();
        }

        public void CloseConnection(DbExport conn)
        {
            if (conn.DbConnection.State == System.Data.ConnectionState.Open)
                conn.DbConnection.Close();
        }
        
       

        private OracleConnectionFactory()
        {
            DbExport[] dbExports = new DbExport[numberConnections];
            ManualResetEvent[] doneEvents = new ManualResetEvent[numberConnections];

            // populate arrays and start threads 
            for (int i = 0; i < numberConnections; i++)
            {
                // initialize each event object in the array 
                doneEvents[i] = new ManualResetEvent(false);
                // create a new instance of the ConnectionThread class 
                DbExport export = new DbExport(i+1, _credentials.User, _credentials.Password, _credentials.Service, doneEvents[i]);
                // assign the new instance to array element 
                dbExports[i] = export;
                // Queue the thread for execution and specify the method to execute 
                // when thread becomes available from the thread pool 
                ThreadPool.QueueUserWorkItem(export.ThreadPoolCallback);
            }
        }

        /// <summary>
        /// Instance - Singleton Implementation
        /// </summary>
        public static OpenStreetMap2Oracle.oracle.OracleConnectionFactory Instance
        {
            get {
                if (_instance == null)
                    _instance = new OpenStreetMap2Oracle.oracle.OracleConnectionFactory();
                return _instance; 
            }            
        }


        /// <summary>
        /// Gets a Free Connection from Connection Pool
        /// </summary> 
        public DbExport GetFreeConnection()
        {
           

        }

     }
}
