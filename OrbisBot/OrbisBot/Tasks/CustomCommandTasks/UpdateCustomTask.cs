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
    class UpdateCustomTask : TaskAbstract
    {
        public UpdateCustomTask(FileBasedTaskPermission permission) : base(permission)
        {
        }

        public override string AboutText()
        {
            return "Appends new return options for the custom command";
        }

        public override bool CheckArgs(string[] args)
        {
            return args.Length == 3;
        }

        public override string CommandText()
        {
            return "commands-update";
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            if (!Context.Instance.Tasks.ContainsKey(args[1]))
            {
                return $"The command {args[1]} does not exist in this channel";
            }

            if (!(Context.Instance.Tasks[args[1]] is CustomTask))
            {
                return $"The command {args[1]} is not a custom command";
            }

            var newOptions = CommandParser.ParseList(args[2]);

            var command = (CustomTask)Context.Instance.Tasks[args[1]];

            if (command.TaskPermission.GetCommandPermissionForChannel(messageSource.Channel.Id) == PermissionLevel.UsageDenied)
            {
                return $"The command {args[1]} does not exist for this channel";
            }

            command.UpdateCommand(newOptions, messageSource);

            return $"Command {args[1]} has been successfully updated";
        }

        public override string UsageText()
        {
            return "(command name) [\"new commands\"]";
        }
    }
}
