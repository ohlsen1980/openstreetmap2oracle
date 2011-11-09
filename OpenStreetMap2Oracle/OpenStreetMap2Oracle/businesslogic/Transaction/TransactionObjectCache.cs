using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenStreetMap2Oracle.core;

namespace OpenStreetMap2Oracle.businesslogic.Transaction
{
    public class TransactionObjectCache
    {
        private static Dictionary<long, Point> _items = new Dictionary<long, Point>();
       

        public static void AddNodeItem(Node n)
        {            
            if (n != null)
                _items.Add(n.Id, n.Point);
        }

        public static Point GetVertice(long id)
        {
            Point retVal = null;

            _items.TryGetValue(id, out retVal);

            return retVal;
        }

        public static void Clear()
        {
            _items.Clear();
        }
    }
}
