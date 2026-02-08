using System.Collections.Generic;
using System.Linq;

namespace WordPress_XML_Tool
{
    public static class ArticleFilter
    {
        public static List<Article> Filter(List<Article> articles, string title, string year, string author)
        {
            return articles.Where(a =>
                (string.IsNullOrEmpty(title) || (a.Title ?? "").ToLower().Contains(title.ToLower())) &&
                (string.IsNullOrEmpty(year) || (a.Date ?? "").StartsWith(year)) &&
                (string.IsNullOrEmpty(author) || (a.Author ?? "") == author)
            ).ToList();
        }
    }
}
