using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using OrbisBot.TaskAbstracts;
using OrbisBot.TaskHelpers.UserFinder;
using HtmlAgilityPack;

namespace OrbisBot.Tasks
{
    class InsultTask : FilePermissionTaskAbstract
    {

        private Dictionary<long, DateTime> _lastTriggered;

        public InsultTask()
        {
            _lastTriggered = new Dictionary<long, DateTime>();
        }
        public override string AboutText()
        {
            return "Insults a person of your choice";
        }

        public override string CommandText()
        {
            return "insult";
        }

        public override CommandPermission DefaultCommandPermission()
        {
            return new CommandPermission(false, PermissionLevel.User, false);
        }

        public override string PermissionFileSource()
        {
            return Constants.INSULT_FILE;
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            if (_lastTriggered.ContainsKey(messageSource.Channel.Id))
            {
                var commandLastTriggered = _lastTriggered[messageSource.Channel.Id];
                if ((DateTime.Now - commandLastTriggered).TotalSeconds < 30)
                {
                    return string.Format("The bot is still amazed at itself spitting out hotfiya, try again in {0:0.00} seconds", (30 - (DateTime.Now - commandLastTriggered).TotalSeconds));
                }
            }
            else
            {
                _lastTriggered.Add(messageSource.Channel.Id, new DateTime(0));
            }

            if (args.Length != 2)
            {
                return $"{Constants.SYNTAX_INTRO} \"<target>\"";
            }

            var person1 = UserFinderUtil.FindUser(messageSource.Server.Members, args[1]);

            if (person1 == null)
            {
                return $"User {args[1]} cannot be found";
            }

            var web = new HtmlWeb();

            var doc = web.Load("http://randominsults.net/");

            var content = Mention.User(person1);

            var pNodes = doc.DocumentNode.Descendants("strong");

            content = content + " - " + pNodes.First().FirstChild.InnerText;

            //add to the timer dictionary
            _lastTriggered[messageSource.Channel.Id] = DateTime.Now;

            return content;
        }
    }
}
