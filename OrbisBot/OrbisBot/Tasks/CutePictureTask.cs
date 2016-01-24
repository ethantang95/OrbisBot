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
using OrbisBot.TaskHelpers.Reddit;
using OrbisBot.TaskAbstracts;

namespace OrbisBot.Tasks
{
    class CutePictureTask : FilePermissionTaskAbstract
    {
        public override string AboutText()
        {
            return "Sends back a cute picture";
        }

        public override bool CheckArgs(string[] args)
        {
            if (args.Length > 2)
            {
                return false;
            }

            if (args.Length == 1)
            {
                return true;
            }

            bool isSupportAnimal = false;
            isSupportAnimal |= args[1].Equals("cat", StringComparison.InvariantCultureIgnoreCase);

            return isSupportAnimal;
        }

        public override string CommandText()
        {
            return "aww";
        }

        public override CommandPermission DefaultCommandPermission()
        {
            return new CommandPermission(false, PermissionLevel.User, false, 30);
        }

        public override string PermissionFileSource()
        {
            return Constants.CUTE_PICTURE_FILE;
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {

            if (args.Length == 1)
            {
                var client = new RestClient("http://reddit.com");
                var request = new RestRequest("r/aww/rising/.json", Method.GET);

                var response = client.Execute(request);

                var redditObj = JObject.Parse(response.Content);

                return RedditRandomHelper.GetRandomLinkFromRedditSource(redditObj, true);
            }
            else if (args[1].Equals("Cat", StringComparison.InvariantCultureIgnoreCase))
            {
                var client = new RestClient("http://thecatapi.com");
                var request = new RestRequest("api/images/get", Method.GET);
                request.AddParameter("format", "src");
                request.AddParameter("type", "jpg");

                var response = client.Execute(request);

                return response.ResponseUri.AbsoluteUri;
            }

            throw new NotSupportedException($"The parameter {args[1]} managed to bypass the args checking filter"); //args checking should've caught everything
        }

        public override string UsageText()
        {
            return "OPTIONAL<cat>";
        }
    }
}
