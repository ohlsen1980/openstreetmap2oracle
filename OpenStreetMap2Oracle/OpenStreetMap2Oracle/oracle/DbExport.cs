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

        public OracleTransaction Transaction
        {
            get;
            set;
        }

        public DbExport(string schemaName, string sPwd, string sDataSource)
        {           
            OracleConnectionStringBuilder oConnectionBuilder = new OracleConnectionStringBuilder();
            oConnectionBuilder.DataSource = sDataSource.Replace("/", @"\"); //Replace the / with a \ (standard path);           
            oConnectionBuilder.Add("Password", sPwd);
            oConnectionBuilder.Add("User ID", schemaName);
            oConnectionBuilder.Pooling = true;
            oConnectionBuilder.MaxPoolSize = 50;
            oConnectionBuilder.MinPoolSize = 50;
            //!!!!!!!!!!!!!!!!!!!!!!!!
            //Unicode=true is much important, so all characters from OSM data are exported right
            //!!!!!!!!!!!!!!!!!!!!!!!!
            oConnectionBuilder.Unicode = true;
            m_sConnectionString = oConnectionBuilder.ConnectionString;            
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
        public OracleConnection DbConnection
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
        public void openDbConnection()
        {
            if (DbConnection.State != ConnectionState.Open)
                DbConnection.Open();
        }

        /// <summary>
        /// Close the connection
        /// </summary>
        public void closeDbConnection()
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
        /// <param name="sSqlCmd"></param>
        public void execSqlCmd(string sSqlCmd)
        {
            openDbConnection();
            using (OracleCommand dbSqlCmd = DbConnection.CreateCommand())
            {
                dbSqlCmd.CommandText = sSqlCmd;
                dbSqlCmd.ExecuteNonQuery();
                dbSqlCmd.Dispose();
            }
        }

        /// <summary>
        /// Execucte a SQL command using the given dbSqlCmd
        /// This is much faster, than execSqlCmd(string sSqlCmd)
        /// </summary>
        /// <param name="sSqlCmd">The sql syntax</param>
        /// <param name="dbSqlCmd">The OracleCommand object</param>
        public void execSqlCmd(string sSqlCmd, OracleCommand dbSqlCmd)
        {
            dbSqlCmd.CommandText = sSqlCmd;           
            dbSqlCmd.ExecuteNonQuery();
            dbSqlCmd.Dispose();
        }

        /// <summary>
        /// Get a node from the POINT Table
        /// This method is needed, when all nodes are exported to oracle and the ways have to be constructed
        /// </summary>
        /// <param name="id">The OSM_ID of the node</param>
        /// <param name="dbSqlCmd">The OracleCommand</param>
        /// <returns>A Point for contruction of a LineString/Way</returns>
        public Point GetNode(long id, OracleCommand dbSqlCmd)
        {
            Point p = null;
            using (dbSqlCmd)
            {
                string sql = string.Format("select m.ora_geometry.sdo_point.X as X, m." + TableNames.GeomColumName + ".sdo_point.Y as Y, m." + TableNames.GeomColumName + ".sdo_srid as srid from {0} m where m.osm_id = {1}", TableNames.PointTable, id.ToString());
                dbSqlCmd.CommandText = sql;
                
                OracleDataReader rdr = dbSqlCmd.ExecuteReader(System.Data.CommandBehavior.Default);
                rdr.Read();
                string x = rdr[0].ToString();
                string y = rdr[1].ToString();
                string srid = rdr[2].ToString();
                p = new Point(x, y, srid);
                rdr.Close();               
            }
            return p;
        }

        /// <summary>
        /// Get the GML Geometry out of oracle for a given OSM_ID
        /// </summary>
        /// <param name="id">The OSM_ID of the element, it can be a point, line, polygon or road</param>
        /// <param name="dbSqlCmd">The OracleCommand</param>
        /// <returns>A String of coordinates</returns>
        public String GetGMLGeometry(long id, OracleCommand dbSqlCmd)
        {
            String gml = String.Empty;
            using (dbSqlCmd)
            {
                string sql = string.Format("select SDO_UTIL.TO_GMLGEOMETRY(m."+TableNames.GeomColumName +") from {0} m where m.osm_id = {1}", TableNames.LineTable, id.ToString());
                dbSqlCmd.CommandText = sql;

                OracleDataReader rdr = dbSqlCmd.ExecuteReader(System.Data.CommandBehavior.Default);
                rdr.Read();
                if (rdr.HasRows)
                {
                    gml = rdr[0].ToString();
                }                
                else
                {
                    rdr.Close();
                    rdr.Dispose();
                    sql = string.Format("select SDO_UTIL.TO_GMLGEOMETRY(m." + TableNames.GeomColumName + ") from {0} m where m.osm_id = {1}", TableNames.PolygonTable, id.ToString());
                    dbSqlCmd.CommandText = sql;

                    rdr = dbSqlCmd.ExecuteReader(System.Data.CommandBehavior.Default);
                    rdr.Read();
                    if (rdr.HasRows)
                        gml = rdr[0].ToString();
                    else
                    {
                        rdr.Close();
                        rdr.Dispose();
                        sql = string.Format("select SDO_UTIL.TO_GMLGEOMETRY(m." + TableNames.GeomColumName + ") from {0} m where m.osm_id = {1}", TableNames.RoadTable, id.ToString());
                        dbSqlCmd.CommandText = sql;                      
                        rdr = dbSqlCmd.ExecuteReader(System.Data.CommandBehavior.Default);
                        rdr.Read();
                        if (rdr.HasRows)
                            gml = rdr[0].ToString();
                        else
                        {
                            rdr.Close();
                            rdr.Dispose();
                            sql = string.Format("select SDO_UTIL.TO_GMLGEOMETRY(m." + TableNames.GeomColumName + ") from {0} m where m.osm_id = {1}", TableNames.PointTable, id.ToString());
                            dbSqlCmd.CommandText = sql;

                            rdr = dbSqlCmd.ExecuteReader(System.Data.CommandBehavior.Default);
                            rdr.Read();
                            if (rdr.HasRows)
                                gml = rdr[0].ToString();
                        }
                    }                   
                }              
                rdr.Close();                
            }
            return gml;
        }
    }
}
