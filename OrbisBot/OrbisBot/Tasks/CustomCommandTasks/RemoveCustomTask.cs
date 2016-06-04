using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using OrbisBot.TaskAbstracts;
using OrbisBot.TaskPermissions;

namespace OrbisBot.Tasks
{
    class RemoveCustomTask : TaskAbstract
    {
        public RemoveCustomTask(FileBasedTaskPermission permission) : base(permission)
        {
        }

        public override string AboutText()
        {
            return "Remove a custom made command";
        }

        public override string CommandText()
        {
            return "commands-remove";
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            if (!Context.Instance.Tasks.ContainsKey(Constants.TRIGGER_CHAR + args[1]))
            {
                return $"Cannot find command {args[1]}";
            }

            var task = Context.Instance.Tasks[Constants.TRIGGER_CHAR + args[1]];

            if (task.GetType() != typeof(CustomTask))
            {
                return "You cannot remove a non custom task";
            }

            if (task.TaskPermission.GetCommandPermissionForChannel(messageSource.Channel.Id) > Context.Instance.ChannelPermission.GetUserPermission(messageSource.Channel.Id, messageSource.User.Id))
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
