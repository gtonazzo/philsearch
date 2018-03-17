using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using philsearch.Models;
using System.Data;
using System.Text;

namespace philsearch.Models
{
    public class ArticlesContext
    {

        public string ConnectionString { get; set; }
        public string SearchArticlesWS { get; set; }
        public string SemanticNetworkWS { get; set; }

        public ArticlesContext(string connectionString, string searchArticlesWS, string semanticNetworkWS)
        {
            this.ConnectionString = connectionString;
            this.SearchArticlesWS = searchArticlesWS;
            this.SemanticNetworkWS = semanticNetworkWS;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public List<Article> GetArticles(string filter)
        {
            List<Article> list = new List<Article>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from articles where art_id in (" + filter + ")", conn);

                using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Article()
                            {
                                ArtId = reader["art_id"].ToString(),
                                Title = reader["title"].ToString(),
                                Abstract = reader["art_abstract"].ToString()
                            });
                        }
                    }
                }
            }

            foreach (Article a in list)
            {
                a.Authors = GetAuthors(a.ArtId);
                a.Categories = GetCategories(a.ArtId);
                a.Features = GetFeatures(a.ArtId);
                a.References = GetReferences(a.ArtId);
            };

            return list;
        }

        public List<String> GetAuthors(string artId)
        {
            List<String> list = new List<String>() { };

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from articles_authors where art_id = '" + artId + "'", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(reader["author"].ToString());
                    }
                }
            }
            return list;
        }

        public List<String> GetCategories(string artId)
        {
            List<String> list = new List<String>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from articles_categories where art_id = '" + artId + "'", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(reader["category"].ToString());
                    }
                }
            }
            return list;
        }

        public List<Article.Reference> GetReferences(string artId)
        {
            List<Article.Reference> list = new List<Article.Reference>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("Select b.* from articles_biblio a inner join biblio b on a.biblio_id = b.biblio_id  where art_id = '" + artId + "'", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Article.Reference()
                        {
                            Id = reader["biblio_id"].ToString(),
                            Year = reader["year"].ToString(),
                            Author = reader["author"].ToString(),
                            Title = reader["title"].ToString(),
                            Journal = reader["journal"].ToString(),
                            Number = reader["number"].ToString(),
                            Volume = reader["volume"].ToString(),
                            Pages = reader["pages"].ToString(),
                            Publisher = reader["publisher"].ToString()

                        });
                    }
                }
            }
            return list;
        }



        public List<Article.Feature> GetFeatures(string artId)
        {
            List<Article.Feature> list = new List<Article.Feature>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from articles_features where art_id = '" + artId + "'", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Article.Feature()
                        {
                            Id = reader["feature"].ToString(),
                            TfIdf = Convert.ToDouble(reader["tfidf"])
                        });
                    }
                }
            }
            return list;
        }

    }
}
