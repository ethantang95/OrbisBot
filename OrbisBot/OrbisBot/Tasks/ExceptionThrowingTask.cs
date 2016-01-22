using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using OrbisBot.TaskAbstracts;

namespace OrbisBot.Tasks
{
    class ExceptionThrowingTask : FilePermissionTaskAbstract
    {
        public override string AboutText()
        {
            return "Throws an exception, used for debugging";
        }

        public override bool CheckArgs(string[] args)
        {
            return true;
        }

        public override string CommandText()
        {
            return "crash";
        }

        public override CommandPermission DefaultCommandPermission()
        {
            return new CommandPermission(false, PermissionLevel.Developer, false);
        }

        public override string PermissionFileSource()
        {
            return Constants.EXCEPTION_TEST_FILE;
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            throw new ArgumentException("This is an exception");
        }

        public override string UsageText()
        {
            return "This command is not intended for you to use";
        }
    }
}
