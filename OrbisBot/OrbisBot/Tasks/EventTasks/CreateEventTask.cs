using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using OrbisBot.TaskAbstracts;
using System.Text.RegularExpressions;

namespace OrbisBot.Tasks.EventTasks
{
    class CreateEventTask : FilePermissionTaskAbstract
    {
        public override string AboutText()
        {
            return "Creates an event which will notify you this message at later time";
        }

        public override bool CheckArgs(string[] args)
        {
            //event should have a name, message, target, time of dispatch, and an optional time of delay
            if (args.Length != 5 || args.Length != 6)
            {
                return false;
            }

            //regex check the date
            var timeRegex = new Regex(@"\d{4}\/\d{2}\/\d{2} \d{2}:\d{2} \w{3}");

            if (!timeRegex.IsMatch(args[5]))
            {
                return false;
            }

            if (args.Length == 6 && !timeRegex.IsMatch(args[5]))
            {
                return false;
            }

            return true;
        }

        public override string CommandText()
        {
            throw new NotImplementedException();
        }

        public override CommandPermission DefaultCommandPermission()
        {
            throw new NotImplementedException();
        }

        public override string PermissionFileSource()
        {
            throw new NotImplementedException();
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            throw new NotImplementedException();
        }

        public override string UsageText()
        {
            throw new NotImplementedException();
        }
    }
}
