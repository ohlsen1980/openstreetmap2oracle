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
using OpenStreetMap2Oracle.businesslogic;

namespace OpenStreetMap2Oracle.eventArgs
{
    /// <summary>
    /// Eventarg to signalize that a OSM Element was analyzed and added
    /// </summary>
    public class OSMAddedEventArg : EventArgs
    {
        private OSMElement _element;

        public OSMElement Element
        {
            get { return _element; }           
        }

        public OSMAddedEventArg(OSMElement element)
        {
            this._element = element;
        }
    }
}
