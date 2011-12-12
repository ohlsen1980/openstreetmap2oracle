using System;

namespace OpenStreetMap2Oracle.tools
{
    /// <summary>
    /// Class to provide the last selected Path to the Application
    /// Singleton Implementation
    /// </summary>
    public class PathProvider
    {
        private static PathProvider _instance;
        private static String _path = "C:\\";

        private PathProvider()
        {
        }

        public static PathProvider Instance()
        {
            if (_instance == null)
                _instance = new PathProvider();
            return _instance;
        }

        /// <summary>
        /// The last selected Path to a file or the standard path C:\
        /// </summary>
        public static String Path
        {
            get
            {
                return _path;
            }
            set
            {
                if (value != null && value != String.Empty)
                    _path = value;
            }
        }
    }
}
