using System;

namespace OpenStreetMap2Oracle.businesslogic
{
    /// <summary>
    /// Represents a Tag og an OSM Element, contains of a Key and a value 
    /// </summary>
    public class Tag
    {
        private TagKey key = null;

        /// <summary>
        /// The Key of the tag
        /// </summary>
        public TagKey Key
        {
            get { return key; }
            set { key = value; }
        }
        private string value = string.Empty;

        /// <summary>
        /// The Value of the tag
        /// </summary>
        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        /// <summary>
        /// Initializes an instance of Tag
        /// </summary>
        /// <param name="key">The Key of the Tag</param>
        /// <param name="value">The Value of the Tag</param>
        public Tag(TagKey key, String value)
        {
            this.key = key;
            //If it is a String, then add ' for SQL inserts
            if (this.Key.IsString() == true)
            {
                value = value.Replace("'", "''");
                this.value = "'" + value + "'";
            }
            else
                this.value = value;
        }
    }
}
