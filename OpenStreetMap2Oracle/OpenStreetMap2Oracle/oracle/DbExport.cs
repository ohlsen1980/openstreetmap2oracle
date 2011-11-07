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
using System.Data;
using OpenStreetMap2Oracle.core;
using OpenStreetMap2Oracle.tools;

namespace OpenStreetMap2Oracle.oracle
{
    /// <summary>
    /// Class to communicate with oracle
    /// </summary>
    public class DbExport : IDisposable
    {
       
        private string m_sConnectionString;
        private OracleConnection m_dbConnection = null;

        /// <summary>
        /// Gets or sets the Transaction object of the current connection
        /// </summary>
        public OracleTransaction Transaction
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a new instance of the DbExport object
        /// </summary>
        /// <param name="schema">Name of the schema in your oracle environment</param>
        /// <param name="password">Password</param>
        /// <param name="datasource">Datasource</param>
        public DbExport(string schema, string password, string datasource)
        {           
            OracleConnectionStringBuilder conn_builder = new OracleConnectionStringBuilder();
            conn_builder.DataSource = datasource.Replace("/", @"\"); //Replace the / with a \ (standard path);           
            conn_builder.Add("Password", password);
            conn_builder.Add("User ID", schema);
            conn_builder.Pooling = true;
            conn_builder.MaxPoolSize = 75;
            conn_builder.MinPoolSize = 75;
            //!!!!!!!!!!!!!!!!!!!!!!!!
            //Unicode=true is much important, so all characters from OSM data are exported right
            //!!!!!!!!!!!!!!!!!!!!!!!!
            conn_builder.Unicode = true;
            m_sConnectionString = conn_builder.ConnectionString;            
        }

        /// <summary>
        /// Disposes connection and free resources
        /// </summary>
        public void Dispose()
        {
            if (null != m_dbConnection)
            {
                m_dbConnection.Close();
                m_dbConnection = null;
            }
        }

        /// <summary>
        /// The DBConnection object to oracle
        /// </summary>
        public OracleConnection Connection
        {
            get
            {
                if (null == m_dbConnection)
                {
                    m_dbConnection = new OracleConnection(m_sConnectionString);
                }
                return m_dbConnection;
            }
        }

        /// <summary>
        /// Open the connection
        /// </summary>
        public void Open()
        {
            if (Connection.State != ConnectionState.Open)          
                    Connection.Open();            
        }

        /// <summary>
        /// Close the connection
        /// </summary>
        public void Close()
        {
            try
            {
                if (null == m_dbConnection)
                    return;
                m_dbConnection.Close();
            }
            catch { }
        }

        /// <summary>
        /// Execucte a SQL command, with open a new connection
        /// </summary>
        /// <param name="query">The SQL Query</param>
        public void Execute(string query)
        {
            Open();
            using (OracleCommand dbSqlCmd = Connection.CreateCommand())
            {
                dbSqlCmd.CommandText = query;
                dbSqlCmd.ExecuteNonQuery();
                dbSqlCmd.Dispose();
            }
        }

        /// <summary>
        /// Execucte a SQL command using the given dbSqlCmd
        /// This is much faster, than execSqlCmd(string sSqlCmd)
        /// </summary>
        /// <param name="query">The sql syntax</param>
        /// <param name="cmd">The OracleCommand object</param>
        public void Execute(string query, OracleCommand cmd)
        {
            cmd.CommandText = query;           
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        /// <summary>
        /// Get a node from the POINT Table
        /// This method is needed, when all nodes are exported to oracle and the ways have to be constructed
        /// </summary>
        /// <param name="id">The OSM_ID of the node</param>
        /// <param name="cmd">The OracleCommand</param>
        /// <returns>A Point for contruction of a LineString/Way</returns>
        public Point GetNode(long id, OracleCommand cmd)
        {
            string query,
                   x,
                   y,
                   srid;

            Point p = null;
            OracleDataReader reader;

            using (cmd)
            {
                query = string.Format(@"SELECT 
                                            m.{0}.sdo_point.X as X, 
                                            m.{0}.sdo_point.Y as Y, 
                                            m.{0}.sdo_srid as srid 
                                        FROM 
                                            {1} m 
                                        WHERE 
                                            m.osm_id = {2}", 
                                            TableNames.GeomColumName,
                                            TableNames.PointTable, 
                                            id.ToString()
                                       );
                cmd.CommandText = query;
                
                reader = cmd.ExecuteReader(System.Data.CommandBehavior.Default);
                reader.Read();

                x = reader[0].ToString();
                y = reader[1].ToString();
                srid = reader[2].ToString();
                p = new Point(x, y, srid);

                reader.Close();               
            }
            return p;
        }

        /// <summary>
        /// Get the GML Geometry out of oracle for a given OSM_ID
        /// </summary>
        /// <param name="id">The OSM_ID of the element, it can be a point, line, polygon or road</param>
        /// <param name="cmd">The OracleCommand</param>
        /// <returns>A String of coordinates</returns>
        public String GetGMLGeometry(long id, OracleCommand cmd)
        {
            string  query,
                    gml = string.Empty;

            string[] tableArray = new string[] {
                    TableNames.LineTable,
                    TableNames.PolygonTable,
                    TableNames.RoadTable,
                    TableNames.PointTable
            };

            OracleDataReader reader;

            using (cmd)
            {
                foreach (string tableName in tableArray)
                {
                    query = string.Format(@"SELECT
                                                SDO_UTIL.TO_GMLGEOMETRY(m.{0})
                                            FROM
                                                {1} m
                                            WHERE
                                                m.osm_id = {2}",
                                            TableNames.GeomColumName,
                                            tableName,
                                            id.ToString());

                    cmd.CommandText = query;
                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        gml = reader[0].ToString();
                        reader.Close();
                        reader.Dispose();
                        break;
                    }

                    reader.Close();
                    reader.Dispose();
                }
            }
            return gml;
        }
    }
}
