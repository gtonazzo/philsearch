using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using philsearch.Models;
using System.Text.RegularExpressions;

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

            Articles result = new Articles();

            Regex rgx = new Regex("[^a-zA-Z0-9 ]");
            String cleanText = rgx.Replace(searchText, " ");

            Search s = new Search();
            s.search = cleanText;

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

            result.SimilarArticlesList = artList;

            List<Article> articles = new List<Article>();
            foreach (SimilarArticles a in artList)
            {
                articles.Add(context.GetArticleInfo(a.Art_Id));
            }

            result.ArticlesList = articles;
            result.Categories = GetCategories(result.ArticlesList);
            result.Features = GetFeatures(result.ArticlesList);
            result.References = GetReferences(result.ArticlesList);
            /*
            string filter = "";
            foreach (SimilarArticles a in artList)
            {
                if (filter.Length > 0) { filter = filter + ","; }
                filter = filter + "'" + a.Art_Id + "'";
                Article test = context.GetArticleInfo(a.Art_Id);
            }
            
            List<Article> articles = context.GetArticles(filter);

            result.ArticlesList = articles;
            
            result.Categories = GetCategories(result.ArticlesList);
            result.Features = GetFeatures(result.ArticlesList);
            result.References = GetReferences(result.ArticlesList);
            */

            String concepts = GetNetworkConcepts(cleanText, result.Features);           
            result.ConceptsNetwork = GetSemanticNetwork(concepts);

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
           
            foreach (Article art in articles)
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
            result.Sort();
            result.Reverse();
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
                            // if (af.TfIdf > f.TfIdf) { f.TfIdf = af.TfIdf; }
                            f.TfIdf += af.TfIdf;
                            f.Frequency += 1;
                            add = false;
                        }
                    }
                    if (add == true)
                    {
                        Feature nf = new Feature();
                        nf.Id = af.Id;
                        nf.TfIdf = af.TfIdf;
                        nf.Frequency = 1;
                        result.Add(nf);
                    }
                }
            }
            result.Sort();
            result.Reverse();
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

        private string GetNetworkConcepts(string cleanText, IEnumerable<Feature> features)
        {
            string result = "";
            String[] words = cleanText.Split(" ");
            int wordCount = 0;
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 2)
                {
                    wordCount++;
                    if (result.Length > 0) { result = result + " "; }
                    result = result + words[i];
                }
            }

            if (wordCount > 4)
            {
                result = "";
                for (int i = 0; i < 3; i++)
                {
                    if (result.Length > 0) { result = result + " "; }
                    result = result + features.ElementAt(i).Id;
                }

            }

            return result;

        }
    }
}