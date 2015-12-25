using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;

namespace OrbisBot.Tasks
{
    class ListCommandsTask : FilePermissionTaskAbstract
    {
        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            //we will give back commands that are fit for the user's role
            var userPermission = Context.Instance.ChannelPermission.GetUserPermission(messageSource.Channel.Id,
                messageSource.User.Id);

            var availableCommands =
                Context.Instance.Tasks.Where(s =>
                    s.Value.GetCommandPermissionForChannel(messageSource.Channel.Id) <= userPermission &&
                    !s.Value.IsCommandDisabled());

            var returnMessage = new StringBuilder().AppendLine($"The commands you have available as a(n) {userPermission} on this channel are:");

            if (!availableCommands.Any())
            {
                returnMessage.AppendLine("You have no commands available to you");
            }
            else
            {
                availableCommands.ToList().ForEach(s => returnMessage.AppendLine($"{s.Value.CommandText()} - {s.Value.AboutText()}"));
            }

            return returnMessage.ToString();
        }

        public override string PermissionFileSource()
        {
            return Constants.LIST_COMMANDS_FILE;
        }

        public override CommandPermission DefaultCommandPermission()
        {
            return new CommandPermission(false, PermissionLevel.RestrictedUser, true);
        }

        public override string CommandText()
        {
            return "Commands";
        }

        public override string AboutText()
        {
            return "Lists all the commands you have access to in this server and its information";
        }
    }
}
