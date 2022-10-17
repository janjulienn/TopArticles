using System.Text.Json;
using System.Text.Json.Serialization;

namespace TopArticles
{
    class ArticlesResponse
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }
        [JsonPropertyName("per_page")]
        public int PerPage { get; set; }
        [JsonPropertyName("total")]
        public int Total { get; set; }
        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }
        [JsonPropertyName("data")]
        public List<Data>? Data { get; set; } = new List<Data>();
    }

    class Data
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }
        [JsonPropertyName("url")]
        public string? Url { get; set; }
        [JsonPropertyName("author")]
        public string? Author { get; set; }
        [JsonPropertyName("num_comments")]
        public int? NumComments { get; set; }
        [JsonPropertyName("story_id")]
        public int? StoryId { get; set; }
        [JsonPropertyName("story_title")]
        public string? StoryTitle { get; set; }
        [JsonPropertyName("story_url")]
        public string? StoryUrl { get; set; }
        [JsonPropertyName("parent_id")]
        public int? ParentId { get; set; }
        [JsonPropertyName("created_at")]
        public double? CreatedAt { get; set; }
    }

    internal class Program
    {
        static readonly HttpClient client = new HttpClient();
        static void Main(string[] args)
        {
            var articles = topArticles(2);
            foreach (var article in articles)
            {
                Console.WriteLine(article);
            }
        }

        static string[] topArticles(int limit)
        {
            // Get all articles
            // Since it's just 41 articles or 5 pages, hardcode number of loop
            List<Data> allArticles = new List<Data>();
            for (int page = 1; page <= 5; page++)
            {
                // Get articles
                HttpResponseMessage response = client.GetAsync($"https://jsonmock.hackerrank.com/api/articles?page={page}").Result;
                if (response.IsSuccessStatusCode)
                {
                    ArticlesResponse? articlesResponse = JsonSerializer.Deserialize<ArticlesResponse>(response.Content.ReadAsStringAsync().Result) ?? new ArticlesResponse();
                    List<Data>? data = articlesResponse.Data;
                    if (data != null)
                    {
                        allArticles.AddRange(data);
                    }
                }
            }

            // Get title, Sort, and take top limit 
            var resultArticles = allArticles
                .Select(a => new
                {
                    Title = a.Title ?? (a.StoryTitle ?? string.Empty),
                    a.NumComments,
                })
                .OrderByDescending(a => a.NumComments)
                .ThenByDescending(a => a.Title)
                .Take(limit)
                .ToList();

            // Return final result
            List<string> result = new List<string>();
            foreach (var article in resultArticles)
            {
                if (article.Title != string.Empty)
                {
                    result.Add(article.Title);
                }
            }
            return result.ToArray();
        }
    }
}