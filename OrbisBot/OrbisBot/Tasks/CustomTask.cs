using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;

namespace OrbisBot.Tasks
{
    class CustomTask : TaskAbstract
    {
        private string _commandText = "test";
        public CustomTask()
        {

        }
        public override string AboutText()
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

        public override string PermissionFileSource()
        {
            throw new NotSupportedException(); //we will not have a permission source for custom command
            //instead, we will have a different file that contains everything about it
        }

        public new void SetPermission(long channelId, PermissionLevel level)
        {
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            throw new NotImplementedException();
        }
    }
}
