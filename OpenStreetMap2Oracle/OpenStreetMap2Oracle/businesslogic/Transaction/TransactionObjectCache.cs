using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenStreetMap2Oracle.core;
using System.Diagnostics;
using System.IO;

namespace OpenStreetMap2Oracle.businesslogic.Transaction
{
    public class TransactionObjectCache
    {
        private static Dictionary<long, Point> _items = new Dictionary<long, Point>();
        private static ProcessStartInfo _mStartInfo;
        private static Process _mProcess;
        private static StreamReader _mReader;
        private static StreamWriter _mWriter;


        public static void Init()
        {
            _mStartInfo = new ProcessStartInfo("memexp.exe");
            _mStartInfo.CreateNoWindow = true;
            _mStartInfo.UseShellExecute = false;
            _mStartInfo.RedirectStandardInput = true;
            _mStartInfo.RedirectStandardOutput = true;

            _mProcess = Process.Start(_mStartInfo);
            _mWriter = _mProcess.StandardInput;
            _mReader = _mProcess.StandardOutput;
        }

        public static void AddNodeItem(Node n)
        {
            /*
            if (n != null)
                _items.Add(n.Id, n.Point);*/

            _mWriter.Write(
                String.Format("PUT\r\n{0}\r\n{1}:{2}\r\n", n.Id, n.Point.X, n.Point.Y));
            _mWriter.Flush();
            
        }

        public static Point GetVertice(long id)
        {
            _mWriter.Write(
                String.Format("GET\r\n{0}\r\n", id));
            _mWriter.Flush();

            string coords = _mReader.ReadLine();

            if (!String.IsNullOrEmpty(coords))
            {
                string[] coords_data = coords.Split(new char[] { ':' });
                Point retVal = new Point(coords_data[0], coords_data[1]);
                return retVal;
            }
            else
            {
                return null;
            }
        }

        public static void Stop()
        {
            _mProcess.Kill();
        }

        public static void Clear()
        {
            _items.Clear();
        }
    }
}
