using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace WordPress_XML_Tool
{
    public static class ArticleExporter
    {
        public static void ExportMarkdown(Article article, string filePath)
        {
            File.WriteAllText(filePath, $"# {article.Title}\n\n*Datum:* {article.Date}\n*Autor:* {article.Author}\n\n{article.HtmlContent}");
        }

        public static void ExportPlain(Article article, string filePath)
        {
            File.WriteAllText(filePath, $"Titel: {article.Title}\nDatum: {article.Date}\nAutor: {article.Author}\n\n{article.HtmlContent}");
        }

        public static void ExportJson(Article article, string filePath)
        {
            var obj = new { article.Title, article.Date, article.Author, article.HtmlContent };
            File.WriteAllText(filePath, JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true }));
        }

        public static void ExportXml(Article article, string filePath)
        {
            File.WriteAllText(filePath, article.RawElement.ToString());
        }

        public static string MakeSafeFileName(string title)
        {
            return Regex.Replace(title ?? "Artikel", "[^\\w\\- ]", "").Trim().Replace(" ", "_");
        }
    }
}
