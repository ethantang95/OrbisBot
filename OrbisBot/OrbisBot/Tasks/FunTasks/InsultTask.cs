﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using OrbisBot.TaskAbstracts;
using OrbisBot.TaskHelpers.UserFinder;
using HtmlAgilityPack;
using OrbisBot.TaskPermissions;

namespace OrbisBot.Tasks
{
    class InsultTask : TaskAbstract
    {
        public InsultTask(FileBasedTaskPermission permission) : base(permission)
        {
        }

        public override string AboutText()
        {
            return "Insults a person of your choice";
        }

        public override bool CheckArgs(string[] args)
        {
            return args.Length == 2;
        }

        public override string CommandText()
        {
            return "insult";
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            var person1 = UserFinderUtil.FindUser(messageSource.Server.Users, args[1], Context.Instance.GlobalSetting.HideList);

            if (person1 == null)
            {
                return $"User {args[1]} cannot be found";
            }

            var web = new HtmlWeb();

            var doc = web.Load("http://randominsults.net/");

            var content = person1.Mention;

            var pNodes = doc.DocumentNode.Descendants("strong");

            content = content + " - " + pNodes.First().FirstChild.InnerText;

            return content;
        }

        public override string UsageText()
        {
            return "(\"person\")";
        }
    }
}
