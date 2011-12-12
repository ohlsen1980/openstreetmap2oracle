using System.Collections.Generic;

namespace OpenStreetMap2Oracle.core
{
    /// <summary>
    /// Class to describe, how polygons are compared
    /// </summary>
    public class PolygonComparer : IComparer<Polygon>
    {
        public int Compare(Polygon x, Polygon y)
        {
            if (x.Area > y.Area)
                return 1;
            if (x.Area < y.Area)
                return -1;
            else
                return 0;
        }
    }
}
