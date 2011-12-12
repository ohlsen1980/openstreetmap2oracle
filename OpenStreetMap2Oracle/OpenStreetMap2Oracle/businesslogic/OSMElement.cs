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
