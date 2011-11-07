/*
 OpenStreetMap2Oracle - A windows application to export OpenStreetMap Data 
                (*.osm - files) to an oracle database
 -------------------------------------------------------------------------------
 Copyright (C) 2011  Christian Möller
 -------------------------------------------------------------------------------
 This program is free software; you can redistribute it and/or
 modify it under the terms of the GNU General Public License
 as published by the Free Software Foundation; either version 2
 of the License, or (at your option) any later version.

 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with this program; if not, write to the Free Software
 Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenStreetMap2Oracle.businesslogic.Transaction
{
    /// <summary>
    /// Object for transaction purposes
    /// </summary>
    public class OSMTransactionObject : ICloneable
    {
        /// <summary>
        /// Gets or sets the query string
        /// </summary>
        public string Query
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a new instance of OSMTransactionObject
        /// </summary>
        /// <param name="query">Query string</param>
        public OSMTransactionObject(string query) {
            this.Query = query;
        }

        public object Clone()
        {
            return new OSMTransactionObject(this.Query);
        }
    }
}
