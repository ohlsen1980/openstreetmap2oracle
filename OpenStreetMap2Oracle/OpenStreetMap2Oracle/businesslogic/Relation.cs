using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OracleClient;
using OpenStreetMap2Oracle.oracle;
using OpenStreetMap2Oracle.core;
using OpenStreetMap2Oracle.tools;

namespace OpenStreetMap2Oracle.businesslogic
{
    /// <summary>
    /// Represents a relation in OpenStreetMap Data
    /// See: http://wiki.openstreetmap.org/wiki/Relations
    /// </summary>
    public class Relation :OSMElement
    {
        private long _id = 0;

        /// <summary>
        /// The OSM_ID
        /// </summary>
        public long Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private Dictionary<String, String> _relationMemberTypeList = new Dictionary<string, string>();
        
        private int _version = 0;

        /// <summary>
        /// The version
        /// </summary>
        public int Version
        {
            get { return _version; }
            set { _version = value; }
        }

        private RelationType _relationType = RelationType.Undefined;

        /// <summary>
        /// Type of the relation, currently supported boundary and multipolygon
        /// </summary>
        public RelationType RelationType
        {
            get { return _relationType; }
            set { _relationType = value; }
        }

        private List<RelationMember> _relationMembers = new List<RelationMember>();

        /// <summary>
        /// List of members of a relation
        /// </summary>
        public List<RelationMember> RelationMembers
        {
            get { return _relationMembers; }
            set { _relationMembers = value; }
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

        /// <summary>
        /// This method creates the SQL insert command for exporting to oracle
        /// </summary>
        /// <returns></returns>
        public override string ToSQL()
        {
            String sql = string.Empty;
            if (RelationType == businesslogic.RelationType.MultiPolygon || RelationType == businesslogic.RelationType.Boundary)
            {

                LineString forPolygon = new LineString();
                MultiPolygon multiPol = new MultiPolygon();
                foreach (RelationMember member in RelationMembers)
                {
                    DbExport conn  = OpenStreetMap2Oracle.oracle.OracleConnectionFactory.Instance.GetFreeConnection();
                    using (OracleCommand dbSqlCmd = conn.DbConnection.CreateCommand())
                    {
                        dbSqlCmd.Transaction = conn.DbConnection.BeginTransaction();
                        String gml = conn.GetGMLGeometry(member.Ref, dbSqlCmd);
                        GMLAnalyzer analyzer = new GMLAnalyzer(gml);
                        Geometry g = analyzer.Analyze();
                        if (g != null)
                        {
                            if (g.GetType() == typeof(LineString))
                            {
                                forPolygon.Append(g as LineString);
                                if (forPolygon.IsPolygon() == true)
                                {
                                    Polygon pol = new Polygon();
                                    
                                    if (member.Role.Equals("inner"))
                                        pol.PolygonType = PolygonType.interior;
                                    pol.BuildFromLineString(forPolygon);
                                    forPolygon = null;
                                    forPolygon = new LineString();
                                    multiPol.Polygons.Add(pol);
                                }
                            }
                            if (g.GetType() == typeof(Polygon))
                            {
                                Polygon pol = g as Polygon;
                                if (member.Role.Equals("inner"))
                                    pol.PolygonType = PolygonType.interior;
                                pol.CheckIntegrity();
                                multiPol.Polygons.Add(pol);
                            }
                        }
                    }
                }
                String varray = multiPol.ToVarray();
                StringBuilder builder = new StringBuilder("declare \n varr sdo_ordinate_array; \n BEGIN \n");
                builder.Append(varray);
                builder.Append("\n insert into ");
                builder.Append(TableNames.PolygonTable);
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
                values.Append(multiPol.ToMDSYSGeometry());
                values.Append(",");
                values.Append("TO_TIMESTAMP ('");
                values.Append(this.Timestamp);
                values.Append("', 'DD.MM.RR HH24:MI:SS'));\n");
                builder.Append(values.ToString());
                builder.Append("END;\n");
                sql = builder.ToString();

            }
            return sql;
        }
    }
}
