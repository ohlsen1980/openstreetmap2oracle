using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenStreetMap2Oracle.businesslogic.Transaction
{
    public class TransactionStats
    {
        public long Lines { get; set; }
        public long Nodes { get; set; }
        public long Polygones { get; set; }
        public long Multipolygons { get; set; }
        public long Errors { get; set; }
        public long AverageIps { get; set; }
        public TimeSpan Duration { get; set; }

        public TransactionStats()
        {
            this.Lines =
                this.Nodes =
                this.Polygones =
                this.Multipolygons =
                this.Errors =
                this.AverageIps =
                0;
            this.Duration = new TimeSpan(0);
        }
    }
}
