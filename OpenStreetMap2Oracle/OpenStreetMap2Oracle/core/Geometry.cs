using System;

namespace OpenStreetMap2Oracle.core
{
    /// <summary>
    /// Abstract implementation of a geometry
    /// </summary>
    public abstract class Geometry
    {
        //SRID of the geometry
        public abstract string SRID{ get; set;}

        //Implement the MDSYS Geometry String
        public abstract String ToMDSYSGeometry();

        //Build Geometry from gml:coordinates
        public abstract void BuildFromCoordsString(String coords);
        
    }
}
