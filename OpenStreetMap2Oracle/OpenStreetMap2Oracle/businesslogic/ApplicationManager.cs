﻿//OpenStreetMap2Oracle - A windows application to export OpenStreetMap Data 
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
using OpenStreetMap2Oracle.businesslogic.Xml;
using System.Threading.Tasks;
using OpenStreetMap2Oracle.controller;

namespace OpenStreetMap2Oracle.businesslogic
{
    /// <summary>
    /// This class is the Main Application Class, which parses the XML
    /// NOTICE
    /// Singleton Implementation
    /// </summary>
    public class ApplicationManager : Singleton<ApplicationManager>
    {
        private OSMElement _currentElement;
             

        public delegate void OnOSMElementAddedHandler(object sender, OSMAddedEventArg e);
        public delegate void XMLFinishedHandler(object sender, XMLFinishedEventArgs e);

        /// <summary>
        /// Occurs whenever a new OSMElement was parsed
        /// </summary>
        //public event OnOSMElementAddedHandler OnOSMElementAdded;

        public OnOSMElementAddedHandler OSMElementDelegate
        {
            get;
            set;
        }

        /// <summary>
        /// Event Handler for XML parsing is finished
        /// </summary>
        public event XMLFinishedHandler OnXMLFinished;
        
        public ApplicationManager()
        {
        }

        /// <summary>
        /// Parses the OSM - File 
        /// </summary>
        /// <param name="File">The Filename with path to the *.osm file</param>
        public void ParseXML(String File)
        {   
            //Use XMLTextReader, this class dos not validate XML, so it is recommended for usage with large XML files
            XmlTextReader reader = new XmlTextReader(File);
            
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: // Der Knoten ist ein Element.
                        #region NodeType Element
                        switch (reader.Name)
                        {
                            case XmlNodeNames.NODE:
                                #region NodeName "node"

                                bool isEmty = (reader.IsEmptyElement) ? true : false;
                                Node node = new Node();
                                _currentElement = node;
                                long tempLong = 0;

                                while (reader.MoveToNextAttribute()) // Lesen der Attribute.
                                {
                                    switch (reader.Name)
                                    {
                                        case "id":
                                            Int64.TryParse(reader.Value, out tempLong);
                                            node.Id = tempLong;
                                            break;
                                        
                                        case "lat":
                                            //Double.TryParse(xmlreader.Value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out tempDouble );
                                            node.Point.Y = reader.Value;
                                            break;
                                        
                                        case "lon":
                                            //Double.TryParse(xmlreader.Value, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out tempDouble);
                                            node.Point.X = reader.Value;
                                            break;
                                        
                                        case "timestamp":
                                            node.Timestamp = DateTime.Parse(reader.Value, System.Globalization.CultureInfo.InvariantCulture);
                                            break;

                                        default:
                                            break;
                                    }

                                    #region :: unused tags

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

                                    #endregion

                                }

                                if (AppManagerController.RUN_SLIM_MODE)
                                {
                                    Transaction.TransactionObjectCache.AddNodeItem(node);
                                }

                                if (isEmty)
                                {
                                    //This is not a real node, so only export it when App runs not in slim mode
                                    if (AppManagerController.RUN_SLIM_MODE == false)
                                    {
                                        if (OSMElementDelegate != null)
                                        {
                                            OSMElementDelegate(this, new OSMAddedEventArg(_currentElement));
                                        }
                                    }
                                }

                                #endregion
                                break;

                            case XmlNodeNames.TAG:
                                #region NodeName "tag"
                                
                                String key = String.Empty, 
                                       value = String.Empty;
                                
                                while (reader.MoveToNextAttribute()) // Lesen der Attribute.
                                {
                                    switch (reader.Name)
                                    {
                                        case "k":
                                            key = reader.Value;
                                            break;

                                        case "v":
                                            value = reader.Value;
                                            break;

                                        default:
                                            break;
                                    }
                                }
                                TagKey tagkey = new TagKey(key);
                                //ein Relationstyp muss herausgefunden werden
                                //which relation is it? currently only multipolygon and boundary supported
                                //see http://wiki.openstreetmap.org/wiki/Relations

                                if (((string)tagkey) == "type" && _currentElement.GetType() == typeof(Relation))
                                {
                                    if (value == "multipolygon")
                                    {
                                        (_currentElement as Relation).RelationType = RelationType.MultiPolygon;
                                    }
                                    if (value == "boundary")
                                    {
                                        (_currentElement as Relation).RelationType = RelationType.Boundary;
                                    }
                                }
                                if (tagkey.IsValid())
                                {
                                    Tag tag = new Tag(tagkey, value);
                                    if (((string)tagkey) == "NAME")
                                    {
                                        Tag tag1 = new Tag(tagkey, value);
                                    }
                                    _currentElement.tagList.Add(tag);
                                }
                                #endregion
                                break;

                            case XmlNodeNames.WAY:
                                #region NodeName "way"
                                Way _way = new Way();
                                _currentElement = _way;
                                tempLong = 0;

                                while (reader.MoveToNextAttribute()) // Lesen der Attribute.
                                {
                                    switch (reader.Name)
                                    {
                                        case "id":
                                             Int64.TryParse(reader.Value, out tempLong);
                                            _way.Id = tempLong;
                                            break;

                                        case "timestamp":
                                            _way.Timestamp = DateTime.Parse(reader.Value, System.Globalization.CultureInfo.InvariantCulture);
                                            break;

                                        default:
                                            break;
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

                                    
                                }
                                #endregion
                                break;

                            case XmlNodeNames.ND:
                                #region NodeName "nd"
                                long nodeRef = 0;
                                while (reader.MoveToNextAttribute()) // Lesen der Attribute.
                                {
                                    if (reader.Name == "ref")
                                    {
                                        Int64.TryParse(reader.Value, out nodeRef);
                                    }
                                }

                                if (_currentElement.GetType() == typeof(Way))
                                {
                                    Way way = _currentElement as Way;
                                    if (AppManagerController.RUN_SLIM_MODE == false)
                                    {
                                        DbExport conn = OpenStreetMap2Oracle.oracle.OracleConnectionFactory.CreateConnection();
                                        using (OracleCommand dbSqlCmd = conn.Connection.CreateCommand())
                                        {
                                            dbSqlCmd.Transaction = conn.Transaction;
                                            Point p = conn.GetNode(nodeRef, dbSqlCmd);
                                            if (p != null)
                                                way.Line.AddVertice(p);
                                        }
                                    }
                                    else
                                    {
                                        //Compute Nodes in RAM                                        
                                        Point p = Transaction.TransactionObjectCache.GetVertice(nodeRef);
                                        if (p != null)
                                            way.Line.AddVertice(p);                                        
                                    }
                                    
                                }
                                #endregion
                                break;

                            case XmlNodeNames.RELATION:
                                #region NodeName "relation"
                                Relation relation = new Relation();
                                _currentElement = relation;
                                tempLong = 0;

                                while (reader.MoveToNextAttribute()) // Lesen der Attribute.
                                {
                                    switch (reader.Name)
                                    {
                                        case "id":
                                            Int64.TryParse(reader.Value, out tempLong);
                                            relation.Id = tempLong;
                                            break;

                                        case "timestamp":
                                            relation.Timestamp = DateTime.Parse(reader.Value, System.Globalization.CultureInfo.InvariantCulture);
                                            break;

                                        default:
                                            break;
                                    }
                                }
                                #endregion
                                break;

                            case XmlNodeNames.MEMBER:
                                #region NodeName "member"
                                RelationMember member = new RelationMember();
                                tempLong = 0;

                                while (reader.MoveToNextAttribute()) // Lesen der Attribute.
                                {
                                    switch (reader.Name)
                                    {
                                        case "type":
                                            member.Type = reader.Value;
                                            break;

                                        case "ref":
                                            Int64.TryParse(reader.Value, out tempLong);
                                            member.Ref = tempLong;
                                            break;

                                        case "role":
                                            member.Role = reader.Value;
                                            break;

                                        default:
                                            break;
                                    }
                                }
                                if (_currentElement.GetType() == typeof(Relation))
                                {
                                    (_currentElement as Relation).RelationMembers.Add(member);
                                }
                                #endregion
                                break;

                            default:
                                break;
                        }
                        #endregion
                        break;

                    case XmlNodeType.EndElement: //Anzeigen des Endes des Elements.
                        #region NodeType EndElement
                        switch (reader.Name)
                        {
                            case XmlNodeNames.NODE:
                                if (OSMElementDelegate != null)
                                {
                                    if (AppManagerController.RUN_SLIM_MODE == true)
                                    {
                                        //export only "real" nodes
                                        if (_currentElement.tagList.Count > 0)
                                            OSMElementDelegate(this, new OSMAddedEventArg(_currentElement));
                                    }
                                    else
                                        OSMElementDelegate(this, new OSMAddedEventArg(_currentElement));
                                }
                               
                                break;
                            case XmlNodeNames.WAY:
                                if (OSMElementDelegate != null)
                                {
                                    OSMElementDelegate(this, new OSMAddedEventArg(_currentElement));
                                }
                                
                                break;
                            case XmlNodeNames.RELATION:
                                if (OSMElementDelegate != null)
                                {
                                    OSMElementDelegate(this, new OSMAddedEventArg(_currentElement));
                                }
                               
                                break;
                        }
                        #endregion
                        break;

                }              

            }
            if (reader.EOF)
            {
                if (OnXMLFinished != null)
                    OnXMLFinished(this, new XMLFinishedEventArgs());
            }
        }

    }
}
