
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
