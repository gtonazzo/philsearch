﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using philsearch.Models;

namespace philsearch.Controllers
{
    public class Search
    {
        public string search { get; set; }
    }

    
    public class ArticlesController : Controller
    {
        
        public IActionResult Index(string searchText)
        {
            Articles result = new Articles();

            if (!String.IsNullOrEmpty(searchText))
            {
                result = SearchArticles(searchText);
            }

            return View(result);
        }


        public string ArticlesJson(string searchText)
        {
            Articles result = SearchArticles(searchText);
            return JsonConvert.SerializeObject(result, Formatting.Indented);
        }

        private Articles SearchArticles(string searchText)
        {
            Search s = new Search();
            s.search = searchText;

            ArticlesContext context = HttpContext.RequestServices.GetService(typeof(philsearch.Models.ArticlesContext)) as ArticlesContext;
            string webService = context.SearchArticlesWS;

            IEnumerable<SimilarArticles> artList;
            using (HttpClient client = new HttpClient())
            {
                MediaTypeWithQualityHeaderValue contentType = new MediaTypeWithQualityHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(contentType);
                string stringData = JsonConvert.SerializeObject(s);
                var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(webService, contentData).Result;
                artList = JsonConvert.DeserializeObject<IEnumerable<SimilarArticles>>(response.Content.ReadAsStringAsync().Result);

            }

            string filter = "";
            foreach (SimilarArticles a in artList)
            {
                if (filter.Length > 0) { filter = filter + ","; }
                filter = filter + "'" + a.Art_Id + "'";
            }


            List<Article> articles = context.GetArticles(filter);

            Articles result = new Articles();

            result.SimilarArticlesList = artList;
            result.ArticlesList = articles;
            result.ConceptsNetwork = GetSemanticNetwork(searchText);
            result.Categories = GetCategories(result.ArticlesList);
            result.Features = GetFeatures(result.ArticlesList);
            result.References = GetReferences(result.ArticlesList);

            return result;
        }

        public SemanticNetwork GetSemanticNetwork(string searchText)
        {

            SemanticNetwork result = new SemanticNetwork();

            Search s = new Search();
            s.search = searchText;

            ArticlesContext context = HttpContext.RequestServices.GetService(typeof(philsearch.Models.ArticlesContext)) as ArticlesContext;
            string webService = context.SemanticNetworkWS;

            using (HttpClient client = new HttpClient())
            {

                MediaTypeWithQualityHeaderValue contentType = new MediaTypeWithQualityHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(contentType);
                string stringData = JsonConvert.SerializeObject(s);
                var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(webService, contentData).Result;
                result = JsonConvert.DeserializeObject<SemanticNetwork>(response.Content.ReadAsStringAsync().Result);

            }

            return result;

        }

        public List<Category> GetCategories(IEnumerable<Article> articles)
        {
            List<Category> result = new List<Category>();

            foreach(Article art in articles)
            {
                foreach(String ac in art.Categories )
                {
                    bool add = true;
                    foreach(Category c in result)
                    {
                        if(ac.Equals(c.Id) == true)
                        {
                            c.Frequency++;
                            add = false;
                        }
                    }
                    if (add == true)
                    {
                        Category nc = new Category();
                        nc.Id = ac;
                        nc.Frequency = 1;
                        result.Add(nc);
                    }
                }
            }
            return result;
        }

        public List<Feature> GetFeatures(IEnumerable<Article> articles)
        {
            List<Feature> result = new List<Feature>();

            foreach (Article art in articles)
            {
                foreach (Article.Feature af in art.Features)
                {
                    bool add = true;
                    foreach (Feature f in result)
                    {
                        if (af.Id.Equals(f.Id) == true)
                        {
                            if (af.TfIdf > f.TfIdf) { f.TfIdf = af.TfIdf; }
                            add = false;
                        }
                    }
                    if (add == true)
                    {
                        Feature nf = new Feature();
                        nf.Id = af.Id;
                        nf.TfIdf = af.TfIdf;
                        result.Add(nf);
                    }
                }
            }
            return result;
        }

        public List<Reference> GetReferences(IEnumerable<Article> articles)
        {
            List<Reference> result = new List<Reference>();

            foreach (Article art in articles)
            {
                foreach (Article.Reference ar in art.References)
                {
                    bool add = true;
                    foreach (Reference r in result)
                    {
                        if (ar.Id.Equals(r.Id) == true)
                        {
                            add = false;
                        }
                    }
                    if (add == true)
                    {
                        Reference nr = new Reference();
                        nr.Id = ar.Id;
                        nr.Author = ar.Author;
                        nr.Year = ar.Year;
                        nr.Title = ar.Title;
                        nr.Journal = ar.Journal;
                        nr.Volume = ar.Volume;                        
                        nr.Number = ar.Number;
                        nr.Pages = ar.Pages;
                        nr.Publisher = ar.Publisher;
                        result.Add(nr);
                    }
                }
            }
            return result;
        }
    }
}