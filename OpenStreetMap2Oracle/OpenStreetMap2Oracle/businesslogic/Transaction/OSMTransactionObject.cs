using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenStreetMap2Oracle.businesslogic.Transaction
{
    public class OSMTransactionObject
    {
        public string Query
        {
            get;
            set;
        }

        public OSMTransactionObject(string query) {
            this.Query = query;
        }
    }
}
