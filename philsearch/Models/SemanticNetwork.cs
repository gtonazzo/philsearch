using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace philsearch.Models
{
    public class SemanticNetwork
    {

        public class Node
        {
            public string id { get; set; }
            public int group { get; set; }
        }

        public class Link
        {
            public string source { get; set; }
            public string target { get; set; }
            public double value { get; set; }
        }


        public List<Node> nodes { get; set; }
        public List<Link> links { get; set; }

    }
}
