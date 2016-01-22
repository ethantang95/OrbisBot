using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using OrbisBot.TaskAbstracts;
using RestSharp;
using System.Xml;
using System.IO;
using HtmlAgilityPack;
using OrbisBot.TaskHelpers.UserFinder;

namespace OrbisBot.Tasks
{
    class FiftyShadeFicTask : FilePermissionTaskAbstract
    {
        private Dictionary<long, DateTime> _lastTriggered;

        public FiftyShadeFicTask()
        {
            _lastTriggered = new Dictionary<long, DateTime>();
        }

        public override string AboutText()
        {
            return "Generates a fifty shades like lewd for 2 lucky people";
        }

        public override bool CheckArgs(string[] args)
        {
            return args.Length == 3;
        }

        public override string CommandText()
        {
            return "generatelewd"; //should change to something better later or nah... lol
        }

        public override CommandPermission DefaultCommandPermission()
        {
            return new CommandPermission(false, PermissionLevel.User, false);
        }

        public override string PermissionFileSource()
        {
            return Constants.FIFTY_SHADES_FIC_FILE;
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {

            if (_lastTriggered.ContainsKey(messageSource.Channel.Id))
            {
                var commandLastTriggered = _lastTriggered[messageSource.Channel.Id];
                if ((DateTime.Now - commandLastTriggered).TotalSeconds < 180)
                {
                    return string.Format("The bot is still too busy being aroused by its magnificant work, try again in {0:0.00} seconds", (180 - (DateTime.Now - commandLastTriggered).TotalSeconds));
                }
            }
            else
            {
                _lastTriggered.Add(messageSource.Channel.Id, new DateTime(0));
            }

            if (args.Length != 3)
            {
                return $"{Constants.USAGE_INTRO} \"<person 1>\" \"<person 2>\"";
            }

            var person1 = UserFinderUtil.FindUser(messageSource.Server.Members, args[1]);

            if (person1 == null)
            {
                return $"User {args[1]} cannot be found";
            }

            var person2 = UserFinderUtil.FindUser(messageSource.Server.Members, args[2]);

            if (person2 == null)
            {
                return $"User {args[2]} cannot be found";
            }

            var web = new HtmlWeb();

            var doc = web.Load("http://www.xwray.com/fiftyshades");

            var content = "";

            var pNodes = doc.DocumentNode.Descendants("p");

            foreach (var pNode in pNodes)
            {
                var text = pNode.NextSibling.InnerText;
                if (!string.IsNullOrEmpty(text) && !string.IsNullOrWhiteSpace(text))
                {
                    content = content + text + "\n";
                }
            }

            //from processing, it looks like the other person is always a male, so male pronouns will represent
            //person 2  
            //person 1 is mentioned either in first person or 2nd person by person 2

            //replacing the first person
            content = content.Replace(" I ", " " + person1.Name + " ");
            content = content.Replace(" me ", " " + person1.Name + " ");
            content = content.Replace(" me.", " " + person1.Name + ".");
            content = content.Replace(" Me ", " " + person1.Name + " ");
            content = content.Replace(" I'm ", " " + person1.Name + " is ");
            content = content.Replace(" my ", " " + person1.Name + "'s ");
            content = content.Replace(" My ", " " + person1.Name + "'s ");

            //we will not replace the 2nd person since it's always in quotes
            
            content = content.Replace(" he ", " " + person2.Name + " ");
            content = content.Replace(" He ", " " + person2.Name + " ");
            content = content.Replace(" he's ", " " + person2.Name + " is ");
            content = content.Replace("He's ", " " + person2.Name + " is ");
            content = content.Replace(" his ", " " + person2.Name + "'s ");
            content = content.Replace("His ", " " + person2.Name + "'s ");
            content = content.Replace(" he'll ", " " + person2.Name + " will ");
            content = content.Replace("He'll ", " " + person2.Name + " will ");
            content = content.Replace(" him ", " " + person2.Name + " ");
            content = content.Replace("Him ", " " + person2.Name + " ");

            //add to the timer dictionary
            _lastTriggered[messageSource.Channel.Id] = DateTime.Now;

            return content;
        }

        public override string UsageText()
        {
            return "(\"person 1\") (\"person 2\")";
        }
    }
}
