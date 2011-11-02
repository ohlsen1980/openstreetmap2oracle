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
using System.Text;
using OpenStreetMap2Oracle.tools;

namespace OpenStreetMap2Oracle.core
{
    /// <summary>
    /// Represents a Point Geometry
    /// </summary>
    public class Point : Geometry
    {
        private string _srid = "8307";
        /// <summary>
        /// The SRID (Spatial Reference ID)
        /// </summary>
        public override string SRID
        {
            get
            {
                return this._srid;
            }
            set
            {
                this._srid = value;
            }
        }


        private string _X = "0";
        /// <summary>
        /// The X Coordinate
        /// </summary>
        public string X
        {
            get { return _X; }
            set { _X = value; }
        }

        private string _Y = "0";
        /// <summary>
        /// The Y Coordinate
        /// </summary>
        public string Y
        {
            get { return _Y; }
            set { _Y = value; }
        }

        /// <summary>
        /// Double representation of String X Coordinate
        /// </summary>
        public double XCoor
        {
            get 
            {
                string temp = X.Replace(".", ",");
                double outdouble = 0;
                Double.TryParse(temp, out outdouble);
                return outdouble; 
            }            
        }

        /// <summary>
        /// Double representation of String Y Coordinate
        /// </summary>
        public double YCoor
        {
            get
            {
                string temp = Y.Replace(".", ",");
                double outdouble = 0;
                Double.TryParse(temp, out outdouble);
                return outdouble;
            }                
        }

        /// <summary>
        /// Straight implementation of a SQL String for construction of a SDO_GEOMETRY for oracle         /// 
        /// </summary>
        /// <returns>The MDSYS.SDO_GEOMETRY SQL String</returns>
        public override string ToMDSYSGeometry()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("MDSYS.SDO_GEOMETRY(3001,");
            builder.Append(_srid);
            builder.Append(",MDSYS.SDO_POINT_TYPE(");
            builder.Append(X);
            builder.Append(",");
            builder.Append(Y);
            builder.Append(",0),null,null)");
            return builder.ToString();
        }

       
        /// <summary>
        /// Deprecated, compares only the strings, now the PointComparer Class is used to compare Points
        /// 
        /// Checks, if this point is the same like an other point
        /// </summary>
        /// <param name="p">The point which has to be compared to this point</param>
        /// <returns>True if x and y coordinates of both points are equal, false if not</returns>
        public bool Equals(Point p)
        {
            bool retVal = false;
            if (p != null)
            {
                if (this._X.Equals(p.X) && this._Y.Equals(p.Y))
                    retVal = true;
            }
            return retVal;
        }

        public Point(string X, string Y)
        {
            this._X = X;
            this._Y = Y;
        }

        public Point(string X, string Y, string srid)
        {
            this._X = X;
            this._Y = Y;
            this._srid = srid;
        }

        public Point()
        {
        }

        /// <summary>
        /// Constructs a point which has the coordinates 0,0
        /// </summary>
        public static Point Zero
        {
            get
            {
                return new Point("0", "0");
            }
        }

        /// <summary>
        /// Builds a Point from String of coordinates
        /// </summary>
        /// <param name="coords">String of coordinates like "X,Y"</param>
        public override void BuildFromCoordsString(string coords)
        {
            StringTokenizer tok = new StringTokenizer(coords, ",");
            int i = 0;
            String tX = String.Empty, tY = String.Empty;
            while (tok.HasMoreTokens())
            {
                if (i == 0)
                {
                    tX = tok.NextToken();
                    i++;
                }
                if (i == 1)
                {
                    tY = tok.NextToken();
                    i++;
                }
                if (i == 2)
                {
                    tok.NextToken();
                    i = 0;
                    this.X = tX;
                    this.Y = tY;
                }
            }
        }
    }
}
