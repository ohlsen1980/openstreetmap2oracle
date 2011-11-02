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


namespace OpenStreetMap2Oracle.tools
{
    /// <summary>
    /// Provides the names of the standard 4 base tables in oracle
    /// Can be changed to match for your setup
    /// </summary>
    public struct TableNames
    {
        /// <summary>
        /// Point Table, see create_PointTable.sql in package sql
        /// </summary>
        public  const string PointTable = "PLANET_OSM_POINT_WGS84";
        /// <summary>
        /// Line Table, see create_LineTable.sql in package sql
        /// </summary>
        public  const string LineTable = "PLANET_OSM_LINE_WGS84";
        /// <summary>
        /// Polygon Table, see create_PolygonTable.sql in package sql
        /// </summary>
        public  const string PolygonTable = "PLANET_OSM_POLYGON_WGS84";
        /// <summary>
        /// Roads Table, see create_RoadsTable.sql in package sql
        /// </summary>
        public  const string RoadTable = "PLANET_OSM_ROADS_WGS84";
        /// <summary>
        /// Name of the geometry column
        /// </summary>
        public  const string GeomColumName = "ORA_GEOMETRY";
    }
}
