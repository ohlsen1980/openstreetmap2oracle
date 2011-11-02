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
    public class OracleConnectionFactory
    {
        private static OpenStreetMap2Oracle.oracle.OracleConnectionFactory _instance = null;
        private static DbExport exportConnection = null;
        /// <summary>
        /// Transaction object for Oracle
        /// </summary>
        public static OracleTransaction Transaction = null;

        private OracleConnectionFactory()
        {
        }

        /// <summary>
        /// Instance - Singleton Implementation
        /// </summary>
        public OpenStreetMap2Oracle.oracle.OracleConnectionFactory Instance
        {
            get {
                if (_instance == null)
                    _instance = new OpenStreetMap2Oracle.oracle.OracleConnectionFactory();
                return _instance; 
            }            
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
        public static DbExport CreateConnection(String user, String passwort, String service)
        {
            exportConnection = new DbExport(user, passwort, service);
            exportConnection.openDbConnection();
            Transaction = exportConnection.DbConnection.BeginTransaction();
            return exportConnection;
        }

     }
}
