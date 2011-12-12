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
                                if (reader.Name.Equals("gml:LineString"))
                                {
                                    g = new LineString();
                                }
                                if (reader.Name.Equals("gml:Polygon"))
                                {
                                    g = new Polygon();
                                }
                                if (reader.Name.Equals("gml:Point"))
                                {
                                    g = new Point();
                                }
                                break;
                            case XmlNodeType.Text:
                                String coors = reader.Value;
                                g.BuildFromCoordsString(coors);
                                break;
                            case XmlNodeType.EndElement:
                                if (reader.Name.Equals("gml:LineString"))
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
