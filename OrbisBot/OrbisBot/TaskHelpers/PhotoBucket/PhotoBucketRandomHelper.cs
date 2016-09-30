using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.TaskHelpers.PhotoBucket
{
    static class PhotoBucketRandomHelper
    {

        public static string GetRandomPicture(string query)
        {
            return GetRandomPicture(query, 0, 100);
        }

        public static string GetRandomPicture(string query, int maxPages)
        {
            return GetRandomPicture(query, 0, maxPages);
        }

        public static string GetRandomPicture(string query, int minPages, int maxPages)
        {
            var web = new HtmlWeb();

            var page = new Random().Next(minPages, maxPages);

            var queryEscaped = query.Replace(" ", "%20"); //really only the space needs to be escaped

            var doc = web.Load($"http://photobucket.com/images/{queryEscaped}?page={page}");

            var pNodes = doc.DocumentNode.Descendants("script");

            pNodes = pNodes.Where(s => s.InnerText.Contains("Pb.Data.add('searchPageCollectionData'"));

            if (pNodes.Count() == 0)
            {
                return $"No images found for query {query}";
            }

            var requestString = pNodes.First().InnerText;

            requestString = requestString.Replace("Pb.Data.add('doPageCollection', true);\nPb.Data.add('searchPageCollectionData',", "");

            requestString = requestString.Replace(");", "");

            var requestJson = JObject.Parse(requestString);

            var searchResults = requestJson["collectionData"]["items"]["objects"];

            var searchLinks = searchResults.Select(s => s["fullsizeUrl"].Value<string>()).ToList();

            return searchLinks[new Random().Next(0, searchLinks.Count)];
        }
    }
}
