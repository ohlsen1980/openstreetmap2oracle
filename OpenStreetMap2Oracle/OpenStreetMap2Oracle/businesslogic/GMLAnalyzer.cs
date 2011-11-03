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
using OpenStreetMap2Oracle.core;
using System.Xml;
using System.IO;

namespace OpenStreetMap2Oracle.businesslogic
{
    /// <summary>
    /// This class analyzes GML Strings
    /// </summary>
    public class GMLAnalyzer
    {
        private String _gml = String.Empty;

        /// <summary>
        /// The GML String
        /// </summary>
        public String Gml
        {
            get { return _gml; }
            set { _gml = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="GML">The GML String to parse</param>
        public GMLAnalyzer(String GML)
        {
            if (String.IsNullOrEmpty(GML) == false)
            {
                this.Gml = GML;
            }
        }

        /// <summary>
        /// This method analyzes the GML String 
        /// </summary>
        /// <returns>Geometry object</returns>
        public Geometry Analyze()
        {
            Geometry g = null;
            if (String.IsNullOrEmpty(Gml)==false)
            {
                XmlTextReader reader = new XmlTextReader(new StringReader(Gml));
                reader.WhitespaceHandling = WhitespaceHandling.None;                
                try
                {
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (reader.Name == "gml:LineString")
                                {
                                    g = new LineString();
                                }
                                if (reader.Name == "gml:Polygon")
                                {
                                    g = new Polygon();
                                }
                                if (reader.Name == "gml:Point")
                                {
                                    g = new Point();
                                }
                                break;
                            case XmlNodeType.Text:
                                String coors = reader.Value;
                                g.BuildFromCoordsString(coors);
                                break;
                            case XmlNodeType.EndElement:
                                if (reader.Name == "gml:LineString")
                                {
                                }
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
            return g;
        }
    }
}
