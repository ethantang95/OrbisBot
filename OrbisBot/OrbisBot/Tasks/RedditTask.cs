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
using OrbisBot.TaskAbstracts;
using Newtonsoft.Json;

namespace OrbisBot.Tasks
{
    class RedditTask : FilePermissionTaskAbstract
    {
        public override string AboutText()
        {
            return "returns a random post from the front page of the subreddit, if no subreddit is specified, returns a random post from reddit. Note, content might or might not be NSFW";
        }

        public override bool CheckArgs(string[] args)
        {
            if (args.Length > 3)
            {
                return false;
            }
            if (args.Length == 3 && !args[2].Equals("image", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }
            return true;
        }

        public override string CommandText()
        {
            return "reddit";
        }

        public override CommandPermission DefaultCommandPermission()
        {
            return new CommandPermission(false, PermissionLevel.User, false, 30);
        }

        public override string PermissionFileSource()
        {
            return Constants.REDDIT_FILE;
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            var imageOnly = false;

            string extensionUrl;
            if (args.Length == 3)
            {
                if (args[2].Equals("image", StringComparison.InvariantCultureIgnoreCase))
                {
                    imageOnly = true;
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

        public override string UsageText()
        {
            return "OPTIONAL(subreddit) OPTIONAL<image>";
        }

        public override string ExceptionMessage(Exception ex, MessageEventArgs eventArgs)
        {
            if (typeof(JsonReaderException) == ex.GetType())
            {
                return "Attempting to fetch reddit contents is interrupted by a known and currently unsolvable bug that happens occasionally, please try the command again";
            }
            else
            {
                return base.ExceptionMessage(ex, eventArgs);
            }
        }
    }
}
