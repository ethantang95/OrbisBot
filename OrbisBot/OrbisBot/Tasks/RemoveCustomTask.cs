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
    class RemoveCustomTask : FilePermissionTaskAbstract
    {
        public override string AboutText()
        {
            return "Remove a custom made command";
        }

        public override string CommandText()
        {
            return "commands-remove";
        }

        public override CommandPermission DefaultCommandPermission()
        {
            return new CommandPermission(false, PermissionLevel.Moderator, false, 1);
        }

        public override string PermissionFileSource()
        {
            return Constants.CUSTOM_COMMAND_FILE;
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            if (!Context.Instance.Tasks.ContainsKey(args[1]))
            {
                return $"Cannot find command {args[1]}, did you forget the - infront of the command?";
            }

            var task = Context.Instance.Tasks[args[1]];

            if (task.GetType() != typeof(CustomTask))
            {
                return "You cannot remove a non custom task";
            }

            if (task.GetCommandPermissionForChannel(messageSource.Channel.Id) > Context.Instance.ChannelPermission.GetUserPermission(messageSource.Channel.Id, messageSource.User.Id))
            {
                return "You do not have permission to remove this custom command or this custom command is not available in your channel";
            }

            ((CustomTask)task).RemoveCommand(messageSource.Channel.Id);

            return $"Successfully removed the command {args[1]} from your channel";
        }

        public override string ExceptionMessage(Exception ex, MessageEventArgs eventArgs)
        {
            return "An error has occured while trying to delete the command";
        }

        public override bool CheckArgs(string[] args)
        {
            return args.Length == 2;
        }

        public override string UsageText()
        {
            return "(command)";
        }
    }
}
