using System;
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
            return result;

        }

        public SemanticNetwork GetSemanticNetwork(string searchText)
        {

            SemanticNetwork semNet = new SemanticNetwork();

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
                semNet = JsonConvert.DeserializeObject<SemanticNetwork>(response.Content.ReadAsStringAsync().Result);

            }

            return semNet;

        }
    }
}