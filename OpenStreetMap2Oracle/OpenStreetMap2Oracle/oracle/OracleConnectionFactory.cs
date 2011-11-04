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
        private static string m_user,
                              m_password,
                              m_service;

        private static int m_pool_size;
        private static int m_iterator;

        private static List<DbExport> m_connection_pool;

        /// <summary>
        /// Initializes the ConnectionFactory
        /// </summary>
        /// <param name="user">Database Username</param>
        /// <param name="password">Database Password</param>
        /// <param name="service">TNS of the oracle instance</param>
        /// <param name="poolSize">Size of the connection pool</param>
        public static void Init(string user, string password, string service, int poolSize = 30)
        {
            m_user = user;
            m_password = password;
            m_service = service;
            m_pool_size = poolSize;
            m_connection_pool = new List<DbExport>();
        }


        /// <summary>
        /// Commits all current pending data on all connections
        /// </summary>
        public static void CommitAll()
        {
            lock (m_connection_pool)
            {

                Parallel.ForEach(m_connection_pool, _handle =>
                {
                    _handle.Transaction.Commit();
                    _handle.closeDbConnection();
                    _handle.openDbConnection();
                    _handle.Transaction = _handle.DbConnection.BeginTransaction();
                });
            }
        }


        /// <summary>
        /// Disconnects all connections
        /// </summary>
        public static void DisconnectAll()
        {
            Parallel.ForEach(m_connection_pool, _handle =>
                {
                    _handle.Transaction.Dispose();
                    _handle.closeDbConnection();
                });
        }

        
        /// <summary>
        /// Returns an instance for a oracle connection
        /// </summary>
        /// <param name="user">Oracle user</param>
        /// <param name="passwort">Password</param>
        /// <param name="service">TNS name of the oracle database</param>
        /// <returns></returns>
        public static DbExport CreateConnection()
        {
            DbExport _handle;

            if (m_connection_pool.Count < (m_pool_size - 1))
            {
                lock (m_connection_pool)
                {
                    _handle = new DbExport(m_user, m_password, m_service);
                    _handle.openDbConnection();
                    _handle.Transaction = _handle.DbConnection.BeginTransaction();
                    m_connection_pool.Add(_handle);
                }
                return _handle;
            }
            else
            {
                m_iterator++;
                if ((m_iterator) > (m_connection_pool.Count - 1))
                {
                    m_iterator = 0;
                }

                return m_connection_pool[m_iterator];

            }
        }
     }
}
