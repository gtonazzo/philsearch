using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace philsearch.Models
{
    public class Feature : IComparable<Feature>
    {
        public int CompareTo(Feature other)
        {
            return this.TfIdf.CompareTo(other.TfIdf);
        }
        public string Id { get; set; }
        public double TfIdf { get; set; }
        public double Frequency { get; set; }

    }
}
