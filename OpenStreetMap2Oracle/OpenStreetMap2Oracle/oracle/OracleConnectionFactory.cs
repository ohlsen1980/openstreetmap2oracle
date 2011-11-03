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

        public static void Init(string user, string password, string service)
        {
            _user = user;
            _password = password;
            _service = service;
        }

        public static DbExport Connection
        {
            get
            {                
                return exportConnection;
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
            DbExport _handle = new DbExport(_user, _password, _service);
            _handle.openDbConnection();
            _handle.Transaction = _handle.DbConnection.BeginTransaction();
            return _handle;
        }

     }
}
