using System.Collections.Generic;
using System.Xml.Linq;

namespace WordPress_XML_Tool
{
    public class Article
    {
        public string Title { get; set; }
        public string Date { get; set; }
        public string Author { get; set; }
        public string HtmlContent { get; set; }
        public XElement RawElement { get; set; }
    }

    public static class ArticleParser
    {
        public static List<Article> ParseFromXml(XDocument xmlDoc)
        {
            var articles = new List<Article>();
            foreach (var item in xmlDoc.Descendants("item"))
            {
                if (item.Element("{http://wordpress.org/export/1.2/}post_type")?.Value != "post")
                    continue;
                articles.Add(new Article
                {
                    Title = item.Element("title")?.Value ?? "",
                    Date = item.Element("{http://wordpress.org/export/1.2/}post_date")?.Value ?? "",
                    Author = item.Element("{http://purl.org/dc/elements/1.1/}creator")?.Value ?? "",
                    HtmlContent = item.Element("{http://purl.org/rss/1.0/modules/content/}encoded")?.Value ?? "",
                    RawElement = item
                });
            }
            return articles;
        }
    }
}
