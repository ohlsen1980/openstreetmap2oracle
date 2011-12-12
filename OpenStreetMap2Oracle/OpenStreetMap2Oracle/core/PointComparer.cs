using System.Collections.Generic;

namespace OpenStreetMap2Oracle.core
{
    /// <summary>
    /// Class to describe, how points are compared
    /// </summary>
    public class PointComparer : IEqualityComparer<Point>
    {
        public bool Equals(Point x, Point y)
        {
            if (x.XCoor == y.XCoor && x.YCoor == y.YCoor)
                return true;
            else
                return false;
        }

        public int GetHashCode(Point obj)
        {
            return 0815;
        }
    }
}
