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
    class ListCommandsTask : FilePermissionTaskAbstract
    {
        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            //we will give back commands that are fit for the user's role
            var userPermission = Context.Instance.ChannelPermission.GetUserPermission(messageSource.Channel.Id,
                messageSource.User.Id);

            var channelMuted = Context.Instance.ChannelPermission.ContainsChannel(messageSource.Channel.Id) ? Context.Instance.ChannelPermission.ChannelPermissions[messageSource.Channel.Id].Muted : false;

            var availableCommands =
                Context.Instance.Tasks.Where(s =>
                    (s.Value.GetCommandPermissionForChannel(messageSource.Channel.Id) <= userPermission) &&
                    !s.Value.IsCommandDisabled() &&
                    (!channelMuted ||
                        (s.Value.OverrideMuting() && userPermission >= PermissionLevel.Admin)));

            var returnMessage = new StringBuilder().AppendLine($"The commands you have available as a(n) {userPermission} on this channel are:");

            if (!availableCommands.Any())
            {
                returnMessage.AppendLine("You have no commands available to you");
            }
            else
            {
                var availableTasks = availableCommands.Select(s => s.Value).ToList();
                availableTasks.Sort();
                availableTasks.ForEach(s => returnMessage.AppendLine($"{s.CommandTrigger()}"));
            }

            returnMessage.AppendLine("Type about after a command to see what it does or usage to see how to use it.");

            return returnMessage.ToString();
        }

        public override string PermissionFileSource()
        {
            return Constants.LIST_COMMANDS_FILE;
        }

        public override CommandPermission DefaultCommandPermission()
        {
            return new CommandPermission(false, PermissionLevel.RestrictedUser, true, 10);
        }

        public override string CommandText()
        {
            return "commands";
        }

        public override string AboutText()
        {
            return "Lists all the commands you have access to in this server and its information";
        }

        public override bool CheckArgs(string[] args)
        {
            return true;
        }

        public override string UsageText()
        {
            return Constants.NO_PARAMS_USAGE;
        }
    }
}
