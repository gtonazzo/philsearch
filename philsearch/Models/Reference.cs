using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace philsearch.Models
{
    public class Reference
    {
        public string Id { get; set; }
        public string Author { get; set; }
        public string Year { get; set; }
        public string Title { get; set; }
        public string Journal { get; set; }
        public string Volume { get; set; }
        public string Number { get; set; }
        public string Pages { get; set; }
        public string Publisher { get; set; }
    }
}
