﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using OrbisBot.TaskAbstracts;

namespace OrbisBot.Tasks
{
    class BotMentionTask : FilePermissionTaskAbstract
    {
        public override string AboutText()
        {
            return "Short intro to the bot when it is mentioned";
        }

        public override bool CheckArgs(string[] args)
        {
            return true;
        }

        public override string CommandText()
        {
            return "bot-mention";
        }

        public override CommandPermission DefaultCommandPermission()
        {
            return new CommandPermission(false, PermissionLevel.User, false, 1);
        }

        public override string PermissionFileSource()
        {
            return Constants.BOT_MENTION_FILE;
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            return $"Hello {messageSource.User.Name}, I am OrbisBot, type -About to learn more about me";
        }

        public override string UsageText()
        {
            return Constants.NO_PARAMS_USAGE;
        }
    }
}