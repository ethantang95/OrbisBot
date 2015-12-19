using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;

namespace OrbisBot.Tasks
{
    class AboutTask : TaskAbstract
    {
        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            //we do not need any args... really... it will just process itself
            var message = new StringBuilder().AppendLine("Hello, my name is OrbisBot. I am a Discord Bot created by Cygnus/Zero2G")
                .AppendLine("I can do many different things, including being your mini personal assistance.")
                .AppendLine("I am optimized for the game DN but can also handle tasks for other games and general purposes.")
                .Append("Currently I am on version ").AppendLine(Constants.APP_VERSION)
                .AppendLine("All my commands are activated with an exclamation mark, like \"!About\".")
                .AppendLine("Type \"!Commands\" to see what I can do")
                .AppendLine("You should also register yourself to me by typing \"!Register\".")
                .AppendLine("If you would like to see the source code, it is available at https://github.com/ethantang95/OrbisBot");

            return message.ToString();
        }

        public override string PermissionFileSource()
        {
            return Constants.ABOUT_SETTINGS_FILE;
        }

        public override CommandPermission DefaultCommands()
        {
            return new CommandPermission(false, PermissionLevel.RestrictedUser, false);
        }

        public override string CommandText()
        {
            return "!About";
        }
    }
}
