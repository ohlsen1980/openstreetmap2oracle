//OpenStreetMap2Oracle - A windows application to export OpenStreetMap Data 
//               (*.osm - files) to an oracle database
//-------------------------------------------------------------------------------
//Copyright (C) 2011  Oliver Schöne
//-------------------------------------------------------------------------------
//This program is free software; you can redistribute it and/or
//modify it under the terms of the GNU General Public License
//as published by the Free Software Foundation; either version 2
//of the License, or (at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program; if not, write to the Free Software
//Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.


using System;
using System.Data.OracleClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace OpenStreetMap2Oracle.oracle
{
    /// <summary>
    /// Class to handle Oracle Connections
    /// Singleton Implementation
    /// </summary>
    public static class OracleConnectionFactory
    {
        private static DbExport exportConnection = null;
        /// <summary>
        /// Transaction object for Oracle
        /// </summary>
        public static OracleTransaction Transaction = null;
        private static string _user,
                              _password,
                              _service;

        private static int _poolSize;

        private static Dictionary<DbExport, bool> _connectionPool;

        public static void Init(string user, string password, string service, int poolSize = 30)
        {
            _user = user;
            _password = password;
            _service = service;
            _poolSize = poolSize;
            _connectionPool = new Dictionary<DbExport, bool>();
        }

        public static DbExport Connection
        {
            get
            {                
                return exportConnection;
            }
        }

        public static void FreeConnection(DbExport connection)
        {
            if (_connectionPool.ContainsKey(connection))
            {
                _connectionPool[connection] = false;
            }
        }


        public static void CommitAll()
        {
            lock (_connectionPool)
            {
                Parallel.ForEach(_connectionPool.Keys, handle =>
                {
                    handle.Transaction.Commit();
                    handle.closeDbConnection();
                    handle.openDbConnection();
                    handle.Transaction = handle.DbConnection.BeginTransaction();
                });
            }
        }

        
        /// <summary>
        /// Creates a new Oracle Connection
        /// </summary>
        /// <param name="user">Oracle user</param>
        /// <param name="passwort">Password</param>
        /// <param name="service">TNS name of the oracle database</param>
        /// <returns></returns>
        public static DbExport CreateConnection()
        {
            DbExport _handle;

            if (_connectionPool.Count < (_poolSize - 1) )
            {
                lock (_connectionPool)
                {
                    _handle = new DbExport(_user, _password, _service);
                    _handle.openDbConnection();
                    _handle.Transaction = _handle.DbConnection.BeginTransaction();
                    _connectionPool.Add(_handle, true);
                }
                return _handle;
            }
            else
            {
                while (true) {
                    lock (_connectionPool)
                    {
                        foreach (DbExport conn in _connectionPool.Keys)
                        {
                            if (!_connectionPool[conn])
                            {
                                _connectionPool[conn] = true;
                                return conn;
                            }
                        }
                    }
                    Thread.Yield();
                }
            }
        }
     }
}
