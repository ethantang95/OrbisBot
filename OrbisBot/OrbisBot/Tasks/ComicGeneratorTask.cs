using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using OrbisBot.TaskAbstracts;
using HtmlAgilityPack;

namespace OrbisBot.Tasks
{
    class ComicGeneratorTask : FilePermissionTaskAbstract
    {
        public override string AboutText()
        {
            return "Generate a random Cyanide and Happiness comic";
        }

        public override bool CheckArgs(string[] args)
        {
            return args.Length == 1;
        }

        public override string CommandText()
        {
            return "generate-comic  ";
        }

        public override CommandPermission DefaultCommandPermission()
        {
            return new CommandPermission(false, PermissionLevel.User, false, 30);
        }

        public override string PermissionFileSource()
        {
            return Constants.RANDOM_COMIC_FILE;
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            var web = new HtmlWeb();

            var doc = web.Load("http://explosm.net/rcg");

            var pNodes = doc.GetElementbyId("rcg-comic");

            var link = "http:" + pNodes.ChildNodes[1].Attributes["src"].Value;

            return link;
        }

        public override string UsageText()
        {
            return Constants.NO_PARAMS_USAGE;
        }
    }
}
