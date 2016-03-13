﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using OrbisBot.TaskAbstracts;
using System.Threading;

namespace OrbisBot.Tasks
{
    class RestartContextTask : DiscretePermissionTaskAbstract
    {
        public override string AboutText()
        {
            return "Recreate the context object... not even sure if this works";
        }

        public override bool CheckArgs(string[] args)
        {
            return true;
        }

        public override string CommandText()
        {
            return "bot-restart";
        }

        public override CommandPermission DefaultCommandPermission()
        {
            return new CommandPermission(false, PermissionLevel.Developer, true, 1);
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            Context.Instance.SignalRestart();
            Context.Instance.DestorySelf();
            Thread.Sleep(2000);

            //for some reason, it exceptons out when the result is returned
            return String.Empty;
        }

        public override string UsageText()
        {
            return "You should not have access to this command";
        }
    }
}
