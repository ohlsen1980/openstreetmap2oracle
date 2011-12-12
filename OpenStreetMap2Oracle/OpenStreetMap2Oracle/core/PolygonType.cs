
namespace OpenStreetMap2Oracle.core
{
    /// <summary>
    /// enum of type int to bind the SDO_GEOMETRY - specific codes for polygons to the polygontype exterior or interior
    /// </summary>
    public enum PolygonType : int
    {
        exterior = 1003,
        interior = 2003
    }
}
