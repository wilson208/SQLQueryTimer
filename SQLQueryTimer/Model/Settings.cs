using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLQueryTimer.Model
{
    public class Settings
    {
        public bool AlwaysOnTop { get; set; }
        public List<Query> Queries { get; set; }
    }
}
