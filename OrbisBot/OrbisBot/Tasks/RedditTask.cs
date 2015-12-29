using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using RestSharp;
using Newtonsoft.Json.Linq;

namespace OrbisBot.Tasks
{
    class RedditTask : FilePermissionTaskAbstract
    {
        public override string AboutText()
        {
            return "returns a random post from the front page of the subreddit, if no subreddit is specified, returns a random post from reddit. Note, content might or might not be NSFW";
        }

        public override string CommandText()
        {
            return "Reddit";
        }

        public override CommandPermission DefaultCommandPermission()
        {
            return new CommandPermission(false, PermissionLevel.User, false);
        }

        public override string PermissionFileSource()
        {
            return Constants.REDDIT_FILE;
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            if (args.Length > 2)
            {
                return $"{Constants.SYNTAX_INTRO} OPTIONAL(<subreddit name>). If no subreddit is specified, it will return something from the front page of reddit";
            }

            string extensionUrl;

            if (args.Length == 2)
            {
                extensionUrl = $"r/{args[1]}/.json";
            }
            else
            {
                extensionUrl = "/.json";
            }
            var client = new RestClient("http://reddit.com");
            var request = new RestRequest(extensionUrl, Method.GET);

            var response = client.Execute(request);

            var redditObj = JObject.Parse(response.Content);

            if (redditObj["data"]["children"].ToList().Count == 0)
            {
                //only way to see if the subreddit exists or not
                return $"The subreddit {args[1]} does not exist or does not have any posts";
            }

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

        private bool IsImageExtension(string url)
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
