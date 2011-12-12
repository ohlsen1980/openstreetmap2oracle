using System;
using System.Text;
using OpenStreetMap2Oracle.core;
using OpenStreetMap2Oracle.tools;

namespace OpenStreetMap2Oracle.businesslogic
{
    /// <summary>
    /// Representation of an osm way in XML
    /// See: http://wiki.openstreetmap.org/wiki/Ways
    /// </summary>
    public class Way : OSMElement
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

        private LineString _line = new LineString();

        /// <summary>
        /// The linestring geometry of the way
        /// </summary>
        public LineString Line
        {
            get { return _line; }
            set { _line = value; }
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

        public Way()
        {

        }

        /// <summary>
        /// Checks if it is a road, currently, no roads are stored in road table, only administrative boundaries, this has to be changed
        /// </summary>
        /// <returns>True if it is a road, false if not</returns>
        public bool IsRoad()
        {
            bool retVal = false;
            foreach (Tag t in tagList)
            {
                if (t.Key.ToString().Equals("boundary"))
                {
                    if (t.Value.Equals("'administrative'"))
                        retVal = true;
                }
            }
            return retVal;
        }

       
        ///
        ///This does not run with large ways, with more than 1000 vertices, because of limitation of size of standrad sql inserts in oracle
        ///
        ///
        //public override string ToSQL()
        //{            
        //    StringBuilder builder = new StringBuilder("insert into ");
        //    if (IsRoad() == false)
        //        builder.Append(TableNames.LineTable);
        //    else
        //        builder.Append(TableNames.RoadTable);
        //    builder.Append(" (osm_id,");
        //    StringBuilder values = new StringBuilder(" values(");
        //    values.Append(this.Id);
        //    values.Append(",");
        //    foreach (Tag tag in tagList)
        //    {
        //        builder.Append(tag.Key);
        //        builder.Append(",");
        //        values.Append(tag.Value);
        //        values.Append(",");
        //    }
        //    builder.Append(TableNames.GeomColumName);
        //    builder.Append(",timestamp)");
        //    values.Append(Line.ToMDSYSGeometry());
        //    values.Append(",");
        //    values.Append("TO_TIMESTAMP ('");
        //    values.Append(this.Timestamp);
        //    values.Append("', 'DD.MM.RR HH24:MI:SS'))");

        //    builder.Append(values.ToString());
        //    return builder.ToString();
        //}

        /// <summary>
        /// This method creates the SQL insert command for exporting to oracle
        /// Here, it uses PL/SQL because of the limitation of size of standard SQL inserts        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToSQL()
        {
            bool isRoad = IsRoad(), isPolyGon = Line.IsPolygon();
            StringBuilder builder = new StringBuilder("declare \n varr sdo_ordinate_array; \n BEGIN \n");
            builder.Append(this.Line.ToVarray());
            builder.Append("\n insert into ");
            if (isRoad == true && isPolyGon == false)
                builder.Append(TableNames.RoadTable);
            if (isPolyGon == true)
                builder.Append(TableNames.PolygonTable);
            else if(isRoad == false && isPolyGon == false)
                builder.Append(TableNames.LineTable);
            builder.Append(" (osm_id,");
            StringBuilder values = new StringBuilder(" values(");
            values.Append(this.Id);
            values.Append(",");
            foreach (Tag tag in tagList)
            {
                builder.Append(tag.Key);
                builder.Append(",");
                values.Append(tag.Value);
                values.Append(",");
            }
            builder.Append(TableNames.GeomColumName);
            builder.Append(",timestamp)");
            if (isPolyGon == false)
            {
                values.Append("MDSYS.SDO_GEOMETRY(3002,");
                values.Append(Line.SRID);
                values.Append(",null,MDSYS.SDO_ELEM_INFO_ARRAY(1,2,1),");
            }
            else
            {
                values.Append("MDSYS.SDO_GEOMETRY(3003,");
                values.Append(Line.SRID);
                values.Append(",null,MDSYS.SDO_ELEM_INFO_ARRAY(1,1003,1),");
            }
            values.Append(" varr)");
            values.Append(",");
            values.Append("TO_TIMESTAMP ('");
            values.Append(this.Timestamp);
            values.Append("', 'DD.MM.RR HH24:MI:SS'));\n");

            builder.Append(values.ToString());
            builder.Append("END;\n");
            return builder.ToString();
        }
    }
}
