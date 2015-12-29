using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.TaskHelpers.Reddit
{
    class RedditRandomHelper
    {
        //gets a random url with a reddit source JObject
        public static string GetRandomLinkFromRedditSource(JObject redditObj)
        {
            var postRoot = redditObj["data"]["children"].ToList().Select(s => s["data"]).ToList();

            List<string> postResults = new List<string>();

            foreach (var postNode in postRoot)
            {
                if (Boolean.Parse(postNode["over_18"].Value<string>()))
                {
                    continue;
                }
                var title = postNode["title"] + ":";
                if (IsImageExtension(postNode["url"].Value<string>()))
                {
                    //get the source for quality purposes
                    postResults.Add(title + "\n" + postNode["preview"]["images"].ToList()[0]["source"]["url"].Value<string>());
                }
                else
                {
                    postResults.Add(title + "\n" + postNode["url"].Value<string>());
                }

            }

            if (postResults.Count == 0)
            {
                return "No valid results has been found";
            }

            return postResults[new Random().Next(0, postResults.Count)];
        }

        private static bool IsImageExtension(string url)
        {
            var isImage = false;
            isImage |= url.Contains("imgur");
            isImage |= url.Contains(".jpg");
            isImage |= url.Contains(".jpeg");
            isImage |= url.Contains(".png");
            isImage |= url.Contains(".gif");

            return isImage;
        }
    }
}
