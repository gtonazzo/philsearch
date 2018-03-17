using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace philsearch.Models
{
    public class Article
    {

        public class Feature
        {
            public string Id { get; set; }
            public double TfIdf { get; set; }
        }

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

        public string ArtId { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string PubName { get; set; }
        public string PubYear { get; set; }
        public string PubInfo { get; set; }
        public string Abstract { get; set; }
        public string Language { get; set; }

        public List<String> Categories { get; set; }
        public List<String> Keywords { get; set; }
        public List<Feature> Features { get; set; }
        public List<String> Authors { get; set; }
        public List<Reference> References { get; set; }

    }
}
