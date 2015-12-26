using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using RestSharp;

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
            var client = new RestClient("http://thecatapi.com");
            var request = new RestRequest("api/images/get", Method.GET);
            request.AddParameter("format", "src");
            request.AddParameter("type", "jpg");

            var response = client.Execute(request);

            return response.ResponseUri.AbsoluteUri;
        }
    }
}
