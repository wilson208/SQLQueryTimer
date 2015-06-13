using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLQueryTimer.Model
{
    public class Query
    {
        public string Name { get; set; }
        public string SqlQuery { get; set; }
        public string ConnectionString { get; set; }
        public long IntervalMilliseconds { get; set; }
        public QueryType QueryType { get; set; }
    }
}
