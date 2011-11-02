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
using System.Collections.Generic;

namespace OpenStreetMap2Oracle.businesslogic
{
    /// <summary>
    /// Abstract implemetation of an OpenStreetMap Element in XML data
    /// </summary>
    public abstract class OSMElement
    {
        /// <summary>
        /// Each element must have a list of tags
        /// </summary>
        public List<Tag> tagList = new List<Tag>();
        
        /// <summary>
        /// Each element must implement the ToSQL String fpr exporting to oracle
        /// </summary>
        /// <returns></returns>
        public abstract String ToSQL();
    }
}
