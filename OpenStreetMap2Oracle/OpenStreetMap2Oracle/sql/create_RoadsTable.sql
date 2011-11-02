--//OpenStreetMap2Oracle - A windows application to export OpenStreetMap Data 
--//               (*.osm - files) to an oracle database
--//-------------------------------------------------------------------------------
--//Copyright (C) 2011  Oliver Schöne
--//-------------------------------------------------------------------------------
--//This program is free software; you can redistribute it and/or
--//modify it under the terms of the GNU General Public License
--//as published by the Free Software Foundation; either version 2
--//of the License, or (at your option) any later version.

--//This program is distributed in the hope that it will be useful,
--//but WITHOUT ANY WARRANTY; without even the implied warranty of
--//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
--//GNU General Public License for more details.

--//You should have received a copy of the GNU General Public License
--//along with this program; if not, write to the Free Software
--//Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.


--This SQL is to create the 4 base tables to store osm data
--It has to be created before the tool exports
create table planet_osm_roads_wgs84(
	OSM_ID number primary key, 
	POWER_SOURCE varchar2(2047), 
        Z_ORDER number, 
        HORSE varchar2(2047), 
        ACCESS_OSM varchar2(2047), 
        ADDR_HOUSENUMBER varchar2(2047), 
		ADDR_STREET varchar2(2047),
        ADDR_CITY varchar2(2047),
        ADDR_FULL varchar2(2047),
        ADDR_POSTCODE varchar2(2047),
        ADDR_COUNTRY varchar2(2047),
        RESIDENCE varchar2(2047), 
        CAPITAL varchar2(2047), 
        OPERATOR_OSM varchar2(2047), 
        ELE varchar2(2047), 
        SPORT varchar2(2047), 
        MILITARY varchar2(2047), 
        POI varchar2(2047), 
        PLACE varchar2(2047), 
        TOURISM varchar2(2047), 
        WOOD varchar2(2047), 
        ADMIN_LEVEL varchar2(2047), 
        AREA varchar2(2047), 
        FOOT varchar2(2047), 
        RELIGION varchar2(2047), 
        ROUTE varchar2(2047), 
        DISUSED varchar2(2047), 
        ADDR_FLATS varchar2(2047), 
        BOUNDARY varchar2(2047), 
        LANDUSE varchar2(2047), 
        LEISURE varchar2(2047), 
        POWER_OSM varchar2(2047), 
        AEROWAY varchar2(2047), 
        EMBANKMENT varchar2(2047), 
        LAYER_OSM varchar2(2047), 
        HIGHWAY varchar2(2047), 
        NATURAL_OSM varchar2(2047), 
        CONSTRUCTION varchar2(2047), 
        CUTTING varchar2(2047), 
        WATERWAY varchar2(2047), 
        REF_OSM varchar2(2047), 
        SERVICE varchar2(2047), 
        LEARNING varchar2(2047), 
        TUNNEL varchar2(2047),         
        BRIDGE varchar2(2047), 
        ONEWAY varchar2(2047), 
        RAILWAY varchar2(2047), 
        WIDTH varchar2(2047), 
        ADDR_INTERPOLATION varchar2(2047), 
        MOTORCAR varchar2(2047), 
        BARRIER varchar2(2047), 
        LOCK_OSM varchar2(2047), 
        SHOP varchar2(2047), 
        AERIALWAY varchar2(2047), 
        JUNCTION varchar2(2047), 
        BUILDING varchar2(2047), 
        BICYCLE varchar2(2047), 
        MAN_MADE varchar2(2047), 
        NAME varchar2(2047), 
        AMENITY varchar2(2047), 
        HISTORIC varchar2(2047), 
        WAY_AREA float, 
        TRACKTYPE varchar2(2047), 
        ORA_GEOMETRY MDSYS.SDO_GEOMETRY,
        timestamp TIMESTAMP);
