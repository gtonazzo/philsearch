using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace philsearch.Models
{
    public class SimilarArticles
    {
        public string Art_Id { get; set; }
        public int Art_Index { get; set; }
        public float Similarity { get; set; }
    }
}
