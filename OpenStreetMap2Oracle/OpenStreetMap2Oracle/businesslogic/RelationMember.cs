using System;

namespace OpenStreetMap2Oracle.businesslogic
{
    /// <summary>
    /// Represents a member of a relation: outer, inner or none
    /// 
    /// </summary>
    public class RelationMember
    {
        private String _type = String.Empty;

        /// <summary>
        /// The type of the member, e.g. way
        /// </summary>
        public String Type
        {
            get { return _type; }
            set { _type = value; }
        }

        private long _ref = 0;

        public long Ref
        {
            get { return _ref; }
            set { _ref = value; }
        }

        private String _role = String.Empty;

        /// <summary>
        /// The role of the member, e.g. outer, inner
        /// </summary>
        public String Role
        {
            get { return _role; }
            set { _role = value; }
        }
    }
}
