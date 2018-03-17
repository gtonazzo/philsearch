﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace philsearch.Models
{
    public class Articles
    {
        public IEnumerable<SimilarArticles> SimilarArticlesList { get; set; }
        public IEnumerable<Article> ArticlesList { get; set; }
        public SemanticNetwork ConceptsNetwork { get; set; }

    }
}