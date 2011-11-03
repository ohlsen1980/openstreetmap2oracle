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
using System.Linq;
using System.Text;
using OpenStreetMap2Oracle.tools;

namespace OpenStreetMap2Oracle.core
{
    /// <summary>
    /// Represents a Linestring Geometry 
    /// </summary>
    public class LineString : Geometry
    {
        public LineString()
        {
        }

        private string _srid = "8307";
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
        }

        /// <summary>
        /// Start point of the Line
        /// </summary>
        public Point StartPoint
        {
            get
            {
                if (_pointList.Count > 0)
                    return _pointList.ElementAt<Point>(0);
                else
                    return null;
            }
        }

        /// <summary>
        /// End point of the Line
        /// </summary>
        public Point EndPoint
        {
            get
            {
                if (_pointList.Count > 1)
                    return _pointList.Last<Point>();
                else
                    return null;
            }
        }

        private List<Point> _pointList = new List<Point>();

        /// <summary>
        /// All vertices of the line
        /// </summary>
        public List<Point> PointList
        {
            get { return _pointList; }
            set { _pointList = value; }
        }

        /// <summary>
        /// Add a vertice to the line 
        /// </summary>
        /// <param name="p">The point which has to be added</param>
        public void AddVertice(Point p)
        {
            this._pointList.Add(p);
        }

        /// <summary>
        /// Remove vertice at number
        /// </summary>
        /// <param name="VerticeNumber">The number of the vertice</param>
        public void RemoveVertice(int VerticeNumber)
        {
            this._pointList.RemoveAt(VerticeNumber);
        }

        /// <summary>
        /// Checks if the linestring is closed and so it is a polygon
        /// </summary>
        /// <returns>True if first vertice = last, otherwise false</returns>
        public bool IsPolygon()
        {
            bool retVal = false;
            if (StartPoint != null && EndPoint != null)
            {
                if (StartPoint.Equals(EndPoint))
                    retVal = true;
            }
            return retVal;
        }

        /// <summary>
        /// Straight implementation of a SQL String for construction of a SDO_GEOMETRY for oracle 
        /// Checks if it is a polygon or not
        /// </summary>
        /// <returns>The MDSYS.SDO_GEOMETRY SQL String</returns>
        public override string ToMDSYSGeometry()
        {
            StringBuilder builder = new StringBuilder();
            if (this.IsPolygon() == false)
            {
                builder.Append("MDSYS.SDO_GEOMETRY(3002,");
                builder.Append(_srid);
                builder.Append(",null,MDSYS.SDO_ELEM_INFO_ARRAY(1,2,1),MDSYS.SDO_ORDINATE_ARRAY(");
                foreach (Point p in _pointList)
                {
                    builder.Append(p.X.Replace(',', '.'));
                    builder.Append(",");
                    builder.Append(p.Y.Replace(',', '.'));
                    builder.Append(",0,");
                }
            }
            else
            {
                Polygon pol = new Polygon();
                pol.BuildFromLineString(this);
                builder.Append("MDSYS.SDO_GEOMETRY(3003,");
                builder.Append(_srid);
                builder.Append(",null,MDSYS.SDO_ELEM_INFO_ARRAY(1,1003,1),MDSYS.SDO_ORDINATE_ARRAY(");
                foreach (Point p in pol.PointList)
                {
                    builder.Append(p.X.Replace(',', '.'));
                    builder.Append(",");
                    builder.Append(p.Y.Replace(',', '.'));
                    builder.Append(",0,");
                }
            }            
            String s = builder.ToString().TrimEnd(',');
            s = s + "))";

            return s;
        }

        /// <summary>
        /// Constructs a varray of all coordinates, this is much faster than standrad sql inserts and you dont have 
        /// limitations of the size
        /// </summary>
        /// <returns>A varray String for PL/SQL insert commands</returns>
        public String ToVarray()
        {
            StringBuilder builder = new StringBuilder("varr := sdo_ordinate_array( ");
            foreach(Point p in _pointList)
            {
                builder.Append(p.X.Replace(',', '.'));
                builder.Append(",");
                builder.Append(p.Y.Replace(',', '.'));
                builder.Append(",0,");                
            }
            String s = builder.ToString().TrimEnd(',');
            s = s + " );";
            return s;
        }

        /// <summary>
        /// Builds a linestring from String of coordinates
        /// </summary>
        /// <param name="coords">String of coordinates like "X1,Y1 X2,Y2 X3,Y3"</param>
        public override void BuildFromCoordsString(string coords)
        {
            this._pointList.Clear();
            StringTokenizer tok = new StringTokenizer(coords, " ");
            int i = 0;
            String X = String.Empty, Y = String.Empty;
            /*
            foreach (string coordPair in tok.Tokens)
            {
                StringTokenizer tok1 = new StringTokenizer(coordPair, ",");
                
            }*/

            while (tok.HasMoreTokens())
            {
                StringTokenizer tok1 = new StringTokenizer(tok.NextToken(), ",");
                while (tok1.HasMoreTokens())
                {
                    if (i == 0)
                    {
                        X = tok1.NextToken();
                        i++;
                    }
                    if (i == 1)
                    {
                        Y = tok1.NextToken();
                        i++;
                    }
                    if (i == 2)
                    {
                        tok1.NextToken();
                        i = 0;
                        this._pointList.Add(new Point(X, Y));
                    }
                }
            }
        }

        /// <summary>
        /// Appends a linestring to this linestring
        /// </summary>
        /// <param name="lineString">The linestring which has to be appended</param>
        public void Append(LineString lineString)
        {
            if (this.IsPolygon() == false)
            {
                foreach (Point p in lineString.PointList)
                    this.AddVertice(p);
            }
        }
    }
}
