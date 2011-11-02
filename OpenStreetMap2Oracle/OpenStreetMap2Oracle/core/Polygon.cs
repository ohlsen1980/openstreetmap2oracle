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
using System.Collections;

namespace OpenStreetMap2Oracle.core
{
    /// <summary>
    /// Represents a Polygon Geometry 
    /// </summary>
    public class Polygon : Geometry
    {
        private double _area = 0;

        private PolygonOrientation _orientation = PolygonOrientation.counterclockwise;
        /// <summary>
        /// Orientation of Polygon clockwise or counterclockwise
        /// </summary>
        public PolygonOrientation Orientation
        {
            get { return _orientation; }
            set { _orientation = value; }
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
                _srid = value;
            }
        }

        private PolygonType _polygonType = PolygonType.exterior;
        /// <summary>
        /// The type of the polygon, standard as single it is a exterior
        /// if it is part of a multipolygon, it can be a interior
        /// </summary>
        public PolygonType PolygonType
        {
            get { return _polygonType; }
            set { _polygonType = value; }
        }

        private List<Point> _pointList = new List<Point>();
        /// <summary>
        /// List of vertices
        /// </summary>
        public List<Point> PointList
        {
            get { return _pointList; }
            set { _pointList = value; }
        }

        /// <summary>
        /// Start point
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
        /// End point
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

        /// <summary>
        /// Checks, if the polygon is closed
        /// </summary>
        /// <returns>True if closed, otherwise false</returns>
        public bool IsClosed()
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
        /// </summary>
        /// <returns>The MDSYS.SDO_GEOMETRY SQL String</returns>
        public override string ToMDSYSGeometry()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("MDSYS.SDO_GEOMETRY(3003,");
            builder.Append(_srid);
            builder.Append(",null,MDSYS.SDO_ELEM_INFO_ARRAY(1," + (int)this.PolygonType + ",1),MDSYS.SDO_ORDINATE_ARRAY(");           

            foreach (Point p in _pointList)
            {
                builder.Append(p.X.Replace(',', '.'));
                builder.Append(",");
                builder.Append(p.Y.Replace(',', '.'));
                builder.Append(",0,");
            }
            //builder.Remove(builder.Length-1, builder.Length);
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
            foreach (Point p in _pointList)
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
        /// Builds a polygon from a Linestring
        /// </summary>
        /// <param name="coords">The Linestring</param>
        public void BuildFromLineString(LineString lineString)
        {
            this.PointList.Clear();
            foreach (Point p in lineString.PointList)
            {
                this.PointList.Add(p);
            }
            CheckIntegrity();
        }

        /// <summary>
        /// The area of the polygon, implemented as sum formula from Gauß
        /// see http://de.wikipedia.org/wiki/Gau%C3%9Fsche_Summenformel
        /// </summary>
        public Double Area
        {
            get
            {
                if (_area == 0)
                {
                    if (IsClosed())
                    {
                        int i = 0;
                        foreach (Point p in PointList)
                        {
                            if(i==0)
                                _area = _area + (p.YCoor * (PointList.Last<Point>().XCoor - PointList[i + 1].XCoor));
                            if(i==PointList.Count-1)
                                _area = _area + (p.YCoor * (PointList[i - 1].XCoor - PointList.First<Point>().XCoor));
                            if (i > 0 && i < PointList.Count-1)
                                _area = _area + (p.YCoor * (PointList[i - 1].XCoor - PointList[i + 1].XCoor));
                            i++;
                        }
                        if (_area < 0)
                        {
                            _area = Math.Abs(_area);
                            this.Orientation = PolygonOrientation.clockwise;
                        }
                        _area = _area / 2;
                    }
                }

                return _area;
            }
        }

        /// <summary>
        /// Do some integrity checks for the polygon
        /// </summary>
        public void CheckIntegrity()
        {
            double area = this.Area;
            //Remove adjacent vertices
            Point start = this.StartPoint;
            Point end = this.EndPoint;
            _pointList.Remove(start);
            _pointList.Remove(end);
            IEnumerable<Point> newPointList = _pointList.Distinct<Point>(new PointComparer());
            _pointList.Clear();
            foreach (Point p in newPointList)
            {
                _pointList.Add(p);
            }
            _pointList.Insert(0, start);
            _pointList.Add(end);
            //check orientation            
            //exterior counter clockwise orientation
            if (this._polygonType == PolygonType.exterior)
            {
                if (this.Orientation == PolygonOrientation.clockwise)
                    _pointList.Reverse();

            }//interior clockwise orientation
            else
            {
                if (this.Orientation == PolygonOrientation.counterclockwise)
                    _pointList.Reverse();
            }           
        }

        /// <summary>
        /// Builds a polygon from String of coordinates
        /// </summary>
        /// <param name="coords">String of coordinates like "X1,Y1 X2,Y2 X3,Y3"</param>
        public override void BuildFromCoordsString(string coords)
        {
            this._pointList.Clear();
            StringTokenizer tok = new StringTokenizer(coords, ",");
            int i = 0;
            String X = String.Empty, Y = String.Empty;
            while (tok.HasMoreTokens())
            {
                if (i == 0)
                {
                    X = tok.NextToken();
                    i++;
                }
                if (i == 1)
                {
                    Y = tok.NextToken();
                    i++;
                }
                if (i == 2)
                {
                    tok.NextToken();
                    i = 0;
                    this._pointList.Add(new Point(X, Y));
                }
            }
            CheckIntegrity();            
        }
    }

  
}
