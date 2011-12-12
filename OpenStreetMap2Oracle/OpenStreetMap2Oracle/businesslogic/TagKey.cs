using System;

namespace OpenStreetMap2Oracle.businesslogic
{
    /// <summary>
    /// Represents a Key of a Tag from OSM Data
    /// </summary>
    public class TagKey
    {
        private string _key = String.Empty;

        /// <summary>
        /// Constructs a Key from a String, which comes from XML parsing
        /// NOTICE
        /// Keys, which names are keywords in oracle will get added a _OSM, e.g. OPERATOR or REF
        /// Character ':' is changed into '_', e.g. ADDR_HOUSENUMBER 
        /// </summary>
        /// <param name="key">The key</param>
        public TagKey(string key)
        {
            key = key.Replace(':', '_');
            key = key.Replace("access", "access_osm");
            key = key.Replace("layer", "layer_osm");
            key = key.Replace("lock", "lock_osm");
            key = key.Replace("natural", "natural_osm");
            key = key.Replace("operator", "operator_osm");
            key = key.Replace("power", "power_osm");
            key = key.Replace("ref", "ref_osm");
            this._key = key;
        }

        public override string ToString()
        {
            return this._key;
        }

        /// <summary>
        /// Checks, if the key is in the enum AllowedTagKeys
        /// </summary>
        /// <returns>True, if it is allowed or false if not</returns>
        public bool IsValid()
        {
            bool retVal = false;
             foreach (string s in Enum.GetNames(typeof(AllowedTagKeys)))
            {
                if (_key.ToUpper().Equals(s))
                    retVal = true;
            }
            return retVal;
        }

        /// <summary>
        /// Checks, if the Tagkey is a String
        /// </summary>
        /// <returns>True if it is a string, false if not</returns>
        public bool IsString()
        {
            bool retVal = true;
            if (this._key.ToUpper().Equals(AllowedTagKeys.Z_ORDER.ToString()) || this._key.ToUpper().Equals(AllowedTagKeys.WAY_AREA.ToString()))
                retVal = false;
            return retVal;
        }
    }
}
