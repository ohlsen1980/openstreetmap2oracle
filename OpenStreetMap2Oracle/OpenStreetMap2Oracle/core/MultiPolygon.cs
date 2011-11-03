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
using System.Text;

namespace OpenStreetMap2Oracle.core
{
    /// <summary>
    /// Represents a Multipolygon Geometry
    /// </summary>
    public class MultiPolygon : Geometry
    {

        public MultiPolygon() :
            base()
        {
        }

        /*private string _srid = "8307";
        /// <summary>
        /// The SRID (Spatial Reference ID)
        /// </summary>
        public override string SRID
        {
            get
            {
                return _srid;
            }
            set
            {
                this._srid = value;
            }
        }*/

        private List<Polygon> _polygons = new List<Polygon>();
        /// <summary>
        /// List of polygons, which are part of this multipolygon
        /// </summary>
        public List<Polygon> Polygons
        {
            get { return _polygons; }
            set { _polygons = value; }
        }

        /// <summary>
        /// Constructs a varray of all coordinates, this is much faster than standrad sql inserts and you dont have 
        /// limitations of the size
        /// </summary>
        /// <returns>A varray String for PL/SQL insert commands</returns>
        public String ToVarray()
        {
            Polygons.Sort(new PolygonComparer());
            StringBuilder builder = new StringBuilder("varr := sdo_ordinate_array( ");
            foreach (Polygon pol in Polygons)
            {
                foreach (Point p in pol.PointList)
                {
                    if (p != null)
                    {
                        if (p.X != null && p.Y != null)
                        {
                            builder.Append(p.X.Replace(',', '.'));
                            builder.Append(",");
                            builder.Append(p.Y.Replace(',', '.'));
                            builder.Append(",0,");
                        }
                    }
                }
            }
            String s = builder.ToString().TrimEnd(',');
            s = s + " );";
            return s;
        }

        /// <summary>
        /// Straight implementation of a SQL String for construction of a SDO_GEOMETRY for oracle 
        /// Checks if a polygon is inner or outer
        /// </summary>
        /// <returns>The MDSYS.SDO_GEOMETRY SQL String</returns>
        public override string ToMDSYSGeometry()
        {
            Polygons.Sort(new PolygonComparer());
            StringBuilder values = new StringBuilder();
            values.Append("MDSYS.SDO_GEOMETRY(3003,");
            values.Append(this.SRID);
            values.Append(",null,MDSYS.SDO_ELEM_INFO_ARRAY(");
            int i = 1;
            foreach (Polygon pol in Polygons)
            {
                if (i == 1)
                {
                    values.Append("1,1003,1,");
                    i = i + pol.PointList.Count;
                }
                else
                {
                    if (pol.PolygonType == PolygonType.interior)
                    {
                        values.Append(i + ",2003,1,");
                        i = i + pol.PointList.Count;
                    }
                    else
                    {
                        values.Append(i + ",1003,1,");
                        i = i + pol.PointList.Count;
                    }
                }
                
            }
            String s = values.ToString().TrimEnd(',');
            s = s + "), varr)";

            
            return s;
        }

        /// <summary>Not implemented
        /// </summary>
        /// <param name="coords">String of coordinates like "X1,Y1 X2,Y2 X3,Y3"</param>
        public override void BuildFromCoordsString(string coords)
        {
            throw new NotImplementedException();
        }
    }
}
