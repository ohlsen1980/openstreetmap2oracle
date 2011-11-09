using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenStreetMap2Oracle.core;
using System.Diagnostics;

namespace OpenStreetMap2Oracle.businesslogic.Transaction
{
    public class TransactionObjectCache
    {
        private static Dictionary<long, Point> _items = new Dictionary<long, Point>();
        private static ProcessStartInfo _mStartInfo;
        private static Process _mProcess;


        public static void Init()
        {
            _mStartInfo = new ProcessStartInfo("memexp.exe");
            _mStartInfo.CreateNoWindow = true;
            _mStartInfo.UseShellExecute = false;
            _mStartInfo.RedirectStandardInput = true;
            _mStartInfo.RedirectStandardOutput = true;

            _mProcess = Process.Start(_mStartInfo);
        }

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
