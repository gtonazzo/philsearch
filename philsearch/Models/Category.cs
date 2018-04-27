using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace philsearch.Models
{
    public class Category : IComparable<Category>
    {
        public int CompareTo(Category other)
        {
            return this.Frequency.CompareTo(other.Frequency);
        }
        public string Id { get; set; }
        public float Frequency { get; set; }
    }
}
