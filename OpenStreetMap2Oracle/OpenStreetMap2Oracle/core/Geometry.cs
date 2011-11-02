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

namespace OpenStreetMap2Oracle.core
{
    /// <summary>
    /// Abstract implementation of a geometry
    /// </summary>
    public abstract class Geometry
    {
        //SRID of the geometry
        public abstract string SRID{ get; set;}

        //Implement the MDSYS Geometry String
        public abstract String ToMDSYSGeometry();

        //Build Geometry from gml:coordinates
        public abstract void BuildFromCoordsString(String coords);
        
    }
}
