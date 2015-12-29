using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using RestSharp;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OrbisBot.Tasks
{
    class CutePictureTask : FilePermissionTaskAbstract
    {
        public override string AboutText()
        {
            return "Sends back a cute picture";
        }

        public override string CommandText()
        {
            return "Aww";
        }

        public override CommandPermission DefaultCommandPermission()
        {
            return new CommandPermission(false, PermissionLevel.User, false);
        }

        public override string PermissionFileSource()
        {
            return Constants.CUTE_PICTURE_FILE;
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            if (args.Length > 2)
            {
                return $"{Constants.SYNTAX_INTRO} OPTIONAL(Cat). Use cat as a parameter for specifically a cat picture, no parameter will serve a random picture from Reddit.com/r/aww/rising";
            }

            if (args.Length == 2 && args[1].Equals("Cat", StringComparison.InvariantCultureIgnoreCase))
            {
                var client = new RestClient("http://thecatapi.com");
                var request = new RestRequest("api/images/get", Method.GET);
                request.AddParameter("format", "src");
                request.AddParameter("type", "jpg");

                var response = client.Execute(request);

                return response.ResponseUri.AbsoluteUri;
            }
            else
            {
                var client = new RestClient("http://reddit.com");
                var request = new RestRequest("r/aww/rising/.json", Method.GET);

                var response = client.Execute(request);

                var redditObj = JObject.Parse(response.Content);

                var imagesRoot = redditObj["data"]["children"].ToList().Select(s => s["data"]["preview"]["images"]).ToList();

                List<string> imageResults = new List<string>();

                foreach (var imageNode in imagesRoot)
                {
                    var resultImage = imageNode.Select(s => s["source"]["url"]).ToList();

                    imageResults.Add(resultImage[0].Value<string>());
                }

                return imageResults[new Random().Next(0, imageResults.Count)];
            }
        }
    }
}
