using System;
using OpenStreetMap2Oracle.businesslogic;

namespace OpenStreetMap2Oracle.eventArgs
{
    /// <summary>
    /// Eventarg to signalize that a OSM Element was analyzed and added
    /// </summary>
    public class OSMAddedEventArg : EventArgs
    {
        private OSMElement _element;

        public OSMElement Element
        {
            get { return _element; }           
        }

        public OSMAddedEventArg(OSMElement element)
        {
            this._element = element;
        }
    }
}
