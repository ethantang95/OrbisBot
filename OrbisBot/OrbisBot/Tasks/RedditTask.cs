using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using RestSharp;
using Newtonsoft.Json.Linq;
using OrbisBot.TaskHelpers.Reddit;

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
            return "reddit";
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
            if (args.Length > 3)
            {
                return $"{Constants.SYNTAX_INTRO} OPTIONAL(<subreddit name>) OPTIONAL(image). If no subreddit is specified, it will return something from the front page of reddit. For a subreddit, you can specifiy if you want an image only or not with the word image after the subreddit's name";
            }
            var imageOnly = false;

            string extensionUrl;
            if (args.Length == 3)
            {
                if (args[2].Equals("image", StringComparison.InvariantCultureIgnoreCase))
                {
                    imageOnly = true;
                }
                else
                {
                    return $"You can only specify whether you want images only or not in the 2nd parameter with the keyword \"image\"";
                }
            }
            if (args.Length >= 2)
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

            if (redditObj["error"] != null || redditObj["data"]["children"].ToList().Count == 0)
            {
                //only way to see if the subreddit exists or not
                return $"The subreddit {args[1]} does not exist or does not have any posts";
            }

            return RedditRandomHelper.GetRandomLinkFromRedditSource(redditObj, imageOnly);

        }
    }
}
