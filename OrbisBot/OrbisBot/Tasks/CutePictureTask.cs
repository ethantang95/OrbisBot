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
            //cat|puppy|dog|red panda|bird|rabbit|koala|fox|squirrel|owl
            bool isSupportAnimal = false;
            isSupportAnimal |= args[1].Equals("cat", StringComparison.InvariantCultureIgnoreCase);
            isSupportAnimal |= args[1].Equals("puppy", StringComparison.InvariantCultureIgnoreCase);
            isSupportAnimal |= args[1].Equals("dog", StringComparison.InvariantCultureIgnoreCase);
            isSupportAnimal |= args[1].Equals("redpanda", StringComparison.InvariantCultureIgnoreCase);
            isSupportAnimal |= args[1].Equals("bird", StringComparison.InvariantCultureIgnoreCase);
            isSupportAnimal |= args[1].Equals("rabbit", StringComparison.InvariantCultureIgnoreCase);
            isSupportAnimal |= args[1].Equals("koala", StringComparison.InvariantCultureIgnoreCase);
            isSupportAnimal |= args[1].Equals("fox", StringComparison.InvariantCultureIgnoreCase);
            isSupportAnimal |= args[1].Equals("squirrel", StringComparison.InvariantCultureIgnoreCase);
            isSupportAnimal |= args[1].Equals("owl", StringComparison.InvariantCultureIgnoreCase);

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
            else if (args[1].Equals("cat", StringComparison.InvariantCultureIgnoreCase))
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
                var subredditString = SubRedditMapper(args[1]);
                var client = new RestClient("http://reddit.com");
                var request = new RestRequest($"r/{subredditString}/new/.json", Method.GET);

                var response = client.Execute(request);

                var redditObj = JObject.Parse(response.Content);

                return RedditRandomHelper.GetRandomLinkFromRedditSource(redditObj, true);
            }
            //args checking should've caught everything
        }

        public override string UsageText()
        {
            return "OPTIONAL<cat|puppy|dog|redpanda|bird|rabbit|koala|fox|squirrel|owl>";
        }

        private string SubRedditMapper(string name)
        {
            name = name.ToLower();
            switch (name)
            {
                case "puppy":
                    return "puppies";
                case "dog":
                    return "dogpictures";
                case "redpanda":
                    return "redpandas";
                case "bird":
                    return "birdpics";
                case "rabbit":
                    return "rabbits";
                case "koala":
                    return "koalas";
                case "fox":
                    return "foxes";
                case "squirrel":
                    return "squirrels";
                case "owl":
                    return "owls";
                default:
                    throw new NotSupportedException($"The parameter {name} managed to bypass the args checking filter");
            }
        }
    }
}
