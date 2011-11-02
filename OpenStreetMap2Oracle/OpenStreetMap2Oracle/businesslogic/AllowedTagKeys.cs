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


namespace OpenStreetMap2Oracle.businesslogic
{
    /// <summary>
    /// This enum lists all currently allowed Tags in OpenStreetMap Data
    /// NOTICE
    /// : is replaced with _ e.g. ADDR_HOUSENUMBER, in names which
    /// are ORACLE keywords, there is added a _OSM e.g. OPERATOR_OSM or REF_OSM 
    /// </summary>
    public enum AllowedTagKeys 
    {
        POWER_SOURCE,
        Z_ORDER,
        HORSE,
        ACCESS_OSM,
        ADDR_HOUSENUMBER,
        RESIDENCE,
        CAPITAL,
        OPERATOR_OSM,
        ELE,
        SPORT,
        MILITARY,
        POI,
        PLACE,
        TOURISM,
        WOOD,
        ADMIN_LEVEL,
        AREA,
        FOOT,
        RELIGION,
        ROUTE,
        DISUSED,
        ADDR_FLATS,
        BOUNDARY,
        LANDUSE,
        LEISURE,
        POWER_OSM,
        AEROWAY,
        EMBANKMENT,
        LAYER_OSM,
        HIGHWAY,
        NATURAL_OSM,
        CONSTRUCTION,
        CUTTING,
        WATERWAY,
        REF_OSM,
        SERVICE,
        LEARNING,
        TUNNEL,
        OSM_ID,
        BRIDGE,
        ONEWAY,
        RAILWAY,
        WIDTH,
        ADDR_INTERPOLATION,
        MOTORCAR,
        BARRIER,
        LOCK_OSM,
        SHOP,
        AERIALWAY,
        JUNCTION,
        BUILDING,
        BICYCLE,
        MAN_MADE,
        NAME,
        AMENITY,
        HISTORIC,
        WAY_AREA,
        TRACKTYPE,
        ORA_GEOMETRY,
        ADDR_STREET,
        ADDR_CITY,
        ADDR_FULL,
        ADDR_POSTCODE,
        ADDR_COUNTRY
    }
}
