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
using System.Linq;
using OpenStreetMap2Oracle.core;
using OpenStreetMap2Oracle.tools;

namespace OpenStreetMap2Oracle.businesslogic
{
    /// <summary>
    /// Representation of an osm node in XML
    /// See: http://wiki.openstreetmap.org/wiki/Nodes
    /// </summary>
    public class Node : OSMElement
    {        
        private long _id = 0;

        /// <summary>
        /// OSM_ID
        /// </summary>
        public long Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private Point _point = Point.Zero;

        /// <summary>
        /// The Point with X;Y
        /// </summary>
        public Point Point
        {
            get { return _point; }
            set { _point = value; }
        }

        private int _version = 0;

        /// <summary>
        /// The version
        /// </summary>
        public int Version
        {
            get { return _version; }
            set { _version = value; }
        }

        private long _changeset = 0;

        /// <summary>
        /// Changeset
        /// </summary>
        public long Changeset
        {
            get { return _changeset; }
            set { _changeset = value; }
        }

        private string _user = string.Empty;

        /// <summary>
        /// User, who created osm dataset
        /// </summary>
        public string User
        {
            get { return _user; }
            set { _user = value; }
        }

        private long _uid = 0;

        /// <summary>
        /// Uid
        /// </summary>
        public long Uid
        {
            get { return _uid; }
            set { _uid = value; }
        }

        private DateTime _timestamp = DateTime.Now;

        /// <summary>
        /// Timestamp of change of dataset
        /// </summary>
        public DateTime Timestamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }

        public Node()
        {

        }

        /// <summary>
        /// This method creates the SQL insert command for exporting to oracle
        /// </summary>
        /// <returns></returns>
        public override string ToSQL()
        {
            StringBuilder builder = new StringBuilder("insert into ");
            builder.Append(TableNames.PointTable);
            builder.Append(" (osm_id,");
            StringBuilder values = new StringBuilder(" values(");
            values.Append(this.Id);
            values.Append(",");

          
            foreach (Tag tag in tagList)
            {
                if (tag.Key == "name")
                {
                    String name = tag.Value;
                }
                builder.Append(tag.Key);
                builder.Append(",");
                values.Append(tag.Value);
                values.Append(",");
            }
            builder.Append(TableNames.GeomColumName);
            builder.Append(",timestamp)");           
            values.Append(Point.ToMDSYSGeometry());
            values.Append(",");
            values.Append("TO_TIMESTAMP ('");
            values.Append(this.Timestamp);
            values.Append("', 'DD.MM.RR HH24:MI:SS'))");            

            builder.Append(values.ToString());
            return builder.ToString();
        }
    }
}
