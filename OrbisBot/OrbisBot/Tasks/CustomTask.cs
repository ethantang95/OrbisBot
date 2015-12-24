using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;

namespace OrbisBot.Tasks
{
    class CustomTask : SingleChannelTaskAbstract
    {
        private string _commandText = "test";
        public CustomTask()
        {

        }
        public override string AboutText()
        {
            throw new NotImplementedException();
        }

        public override bool AllowTaskExecution(MessageEventArgs messageEventArgs)
        {
            throw new NotImplementedException();
        }

        public override string CommandText()
        {
            return _commandText;
        }

        public override CommandPermission DefaultCommands()
        {
            return new CommandPermission(false, PermissionLevel.User, false);
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            throw new NotImplementedException();
        }
    }
}
