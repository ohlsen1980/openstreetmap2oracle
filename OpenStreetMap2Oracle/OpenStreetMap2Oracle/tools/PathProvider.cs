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

namespace OpenStreetMap2Oracle.tools
{
    /// <summary>
    /// Class to provide the last selected Path to the Application
    /// Singleton Implementation
    /// </summary>
    public class PathProvider : OpenStreetMap2Oracle.core.Singleton<PathProvider>
    {
        private static String _path = "C:\\";

        public PathProvider()
        {
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
