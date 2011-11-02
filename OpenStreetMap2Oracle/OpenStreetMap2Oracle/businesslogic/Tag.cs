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
