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
using System.Xml;
using OpenStreetMap2Oracle.eventArgs;
using OpenStreetMap2Oracle.oracle;
using System.Data.OracleClient;
using OpenStreetMap2Oracle.core;

namespace OpenStreetMap2Oracle.businesslogic
{
    /// <summary>
    /// This class is the Main Application Class, which parses the XML
    /// NOTICE
    /// Singleton Implementation
    /// </summary>
    public class ApplicationManager : Singleton<ApplicationManager>
    {
        //static ApplicationManager _instance;
        private OSMElement _currentElement;
             

        public delegate void OnOSMElementAddedHandler(object sender, OSMAddedEventArg e);
        public event OnOSMElementAddedHandler OnOSMElementAdded;

        public delegate void XMLFinishedHandler(object sender, XMLFinishedEventArgs e);
        /// <summary>
        /// Event Handler for XML parsing is finished
        /// </summary>
        public event XMLFinishedHandler OnXMLFinished;
        
        private ApplicationManager()
        {
        }

      /*  /// <summary>
        /// Singleton Instance of Application Manager
        /// </summary>
        /// <returns></returns>
        public static ApplicationManager Instance()
        {
            if (_instance == null)
                _instance = new ApplicationManager();
            return _instance;
        }*/

        /// <summary>
        /// Parses the OSM - File 
        /// </summary>
        /// <param name="File">The Filename with path to the *.osm file</param>
        public void ParseXML(String File)
        {   
            //Use XMLTextReader, this class dos not validate XML, so it is recommended in use of large XML files
            XmlTextReader xmlreader = new XmlTextReader(File);
            
            while (xmlreader.Read())
            {
                switch (xmlreader.NodeType)
                {
                    case XmlNodeType.Element: // Der Knoten ist ein Element.
                        if(xmlreader.Name.Equals("node"))
                        {
                            bool isEmty = false;
                            if (xmlreader.IsEmptyElement == true)
                                isEmty = true;
                            Node node = new Node();                            
                            _currentElement = node;                                                        
                            long tempLong = 0;                          
                            
                            while (xmlreader.MoveToNextAttribute()) // Lesen der Attribute.
                            {                                
                                if (xmlreader.Name.Equals("id"))
                                {
                                    Int64.TryParse(xmlreader.Value, out tempLong);
                                    node.Id = tempLong;                                  
                                }

                                if (xmlreader.Name.Equals("lat"))
                                {
                                    //Double.TryParse(xmlreader.Value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out tempDouble );
                                    node.Point.Y = xmlreader.Value;
                                }

                                if (xmlreader.Name.Equals("lon"))
                                {
                                    //Double.TryParse(xmlreader.Value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out tempDouble);
                                    node.Point.X = xmlreader.Value;
                                }

                                // This Tags will not be used in database

                                //if (xmlreader.Name.Equals("version"))
                                //{
                                //    Int32.TryParse(xmlreader.Value, out tempInt);
                                //    node.Version = tempInt;
                                //}

                                //if (xmlreader.Name.Equals("changeset"))
                                //{
                                //    Int64.TryParse(xmlreader.Value, out tempLong);
                                //    node.Changeset = tempLong;
                                //}

                                //if (xmlreader.Name.Equals("user"))
                                //{
                                //    node.User = xmlreader.Value;
                                //}

                                //if (xmlreader.Name.Equals("uid"))
                                //{
                                //    Int64.TryParse(xmlreader.Value, out tempLong);
                                //    node.Uid = tempLong;
                                //}

                                if (xmlreader.Name.Equals("timestamp"))
                                {                                   
                                    node.Timestamp = DateTime.Parse(xmlreader.Value, System.Globalization.CultureInfo.InvariantCulture);
                                }
                            }
                           
                            if (isEmty == true)
                            {
                                if (OnOSMElementAdded != null)
                                    OnOSMElementAdded(this, new OSMAddedEventArg(_currentElement));
                            }
                        }

                        if (xmlreader.Name.Equals("tag"))
                        {
                            String key = String.Empty, value = String.Empty;                            
                            while (xmlreader.MoveToNextAttribute()) // Lesen der Attribute.
                            {

                                if (xmlreader.Name.Equals("k"))
                                {
                                    key = xmlreader.Value;
                                }
                                if (xmlreader.Name.Equals("v"))
                                {
                                    value = xmlreader.Value;
                                }                                
                            }
                            TagKey tagkey = new TagKey(key);
                            //ein Relationstyp muss herausgefunden werden
                            //which relation is it? currently only multipolygon and boundary supported
                            //see http://wiki.openstreetmap.org/wiki/Relations

                            if (tagkey.ToString().Equals("type")&& _currentElement.GetType()==typeof(Relation))
                            {
                                if (value.Equals("multipolygon"))
                                {
                                    (_currentElement as Relation).RelationType = RelationType.MultiPolygon;
                                }
                                if (value.Equals("boundary"))
                                {
                                    (_currentElement as Relation).RelationType = RelationType.Boundary;
                                }
                            }
                            if (tagkey.IsValid())
                            {
                                Tag tag = new Tag(tagkey, value);
                                if (tagkey.ToString().Equals("NAME"))
                                {
                                    Tag tag1 = new Tag(tagkey, value);
                                }
                                _currentElement.tagList.Add(tag);
                            }
                        }

                        if (xmlreader.Name.Equals("way"))
                        {
                            Way way = new Way();
                            _currentElement = way;                          
                            long tempLong = 0;                           
                            
                            while (xmlreader.MoveToNextAttribute()) // Lesen der Attribute.
                            {
                                if (xmlreader.Name.Equals("id"))
                                {
                                    Int64.TryParse(xmlreader.Value, out tempLong);
                                    way.Id = tempLong;
                                }                        

                                // This Tags will not be used in database

                                //if (xmlreader.Name.Equals("version"))
                                //{
                                //    Int32.TryParse(xmlreader.Value, out tempInt);
                                //    way.Version = tempInt;
                                //}

                                //if (xmlreader.Name.Equals("changeset"))
                                //{
                                //    Int64.TryParse(xmlreader.Value, out tempLong);
                                //    way.Changeset = tempLong;
                                //}

                                //if (xmlreader.Name.Equals("user"))
                                //{
                                //    way.User = xmlreader.Value;
                                //}

                                //if (xmlreader.Name.Equals("uid"))
                                //{
                                //    Int64.TryParse(xmlreader.Value, out tempLong);
                                //    way.Uid = tempLong;
                                //}

                                if (xmlreader.Name.Equals("timestamp"))
                                {
                                    way.Timestamp = DateTime.Parse(xmlreader.Value, System.Globalization.CultureInfo.InvariantCulture);
                                }
                            }                      
                        }

                        if (xmlreader.Name.Equals("nd"))
                        {
                            long nodeRef = 0;                            
                            while (xmlreader.MoveToNextAttribute()) // Lesen der Attribute.
                            {
                                if (xmlreader.Name.Equals("ref"))
                                {
                                    Int64.TryParse(xmlreader.Value, out nodeRef);                                    
                                }  
                            }

                            if(_currentElement.GetType() == typeof(Way))
                            {
                                Way way = _currentElement as Way;
                                using (OracleCommand dbSqlCmd = OpenStreetMap2Oracle.oracle.OracleConnectionFactory.Connection.DbConnection.CreateCommand())
                                {
                                    dbSqlCmd.Transaction = OracleConnectionFactory.Transaction;
                                    way.Line.AddVertice(OpenStreetMap2Oracle.oracle.OracleConnectionFactory.Connection.GetNode(nodeRef, dbSqlCmd));
                                }
                            }

                        }

                        if (xmlreader.Name.Equals("relation"))
                        {
                            Relation relation = new Relation();
                            _currentElement = relation;
                            long tempLong = 0;
                            while (xmlreader.MoveToNextAttribute()) // Lesen der Attribute.
                            {
                                if (xmlreader.Name.Equals("id"))
                                {
                                    Int64.TryParse(xmlreader.Value, out tempLong);
                                    relation.Id = tempLong;                                    
                                }
                                if (xmlreader.Name.Equals("timestamp"))
                                {
                                    relation.Timestamp = DateTime.Parse(xmlreader.Value, System.Globalization.CultureInfo.InvariantCulture);
                                }
                            }
                        }

                        if (xmlreader.Name.Equals("member"))
                        {
                            RelationMember member = new RelationMember();
                            long tempLong = 0;
                            while (xmlreader.MoveToNextAttribute()) // Lesen der Attribute.
                            {
                                if (xmlreader.Name.Equals("type"))
                                {
                                    member.Type = xmlreader.Value;
                                }
                                if (xmlreader.Name.Equals("ref"))
                                {
                                    Int64.TryParse(xmlreader.Value, out tempLong);
                                    member.Ref = tempLong;
                                }
                                if (xmlreader.Name.Equals("role"))
                                {
                                    member.Role = xmlreader.Value;
                                }
                            }
                            if (_currentElement.GetType() == typeof(Relation))
                            {
                                (_currentElement as Relation).RelationMembers.Add(member);
                            }
                        }
                        break;
                  
                    case XmlNodeType.EndElement: //Anzeigen des Endes des Elements.
                        if (xmlreader.Name.Equals("node"))
                        {
                            if (OnOSMElementAdded != null)
                                OnOSMElementAdded(this, new OSMAddedEventArg(_currentElement));
                        }
                        if (xmlreader.Name.Equals("way"))
                        {
                            if (OnOSMElementAdded != null)
                                OnOSMElementAdded(this, new OSMAddedEventArg(_currentElement));
                        }

                        if (xmlreader.Name.Equals("relation"))
                        {
                            if (OnOSMElementAdded != null)
                                OnOSMElementAdded(this, new OSMAddedEventArg(_currentElement));
                        }
                        
                        break;
                }              

            }
            if (xmlreader.EOF)
            {
                if (OnXMLFinished != null)
                    OnXMLFinished(this, new XMLFinishedEventArgs());
            }
        }

    }
}
