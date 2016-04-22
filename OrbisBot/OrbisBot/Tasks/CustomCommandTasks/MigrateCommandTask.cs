using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using OrbisBot.TaskAbstracts;
using OrbisBot.TaskHelpers.CustomCommands;

namespace OrbisBot.Tasks
{
    class MigrateCommandTask : FilePermissionTaskAbstract
    {
        public override string AboutText()
        {
            return "Copy a custom command to a different channel on the server";
        }

        public override bool CheckArgs(string[] args)
        {
            return args.Length == 3;
        }

        public override string CommandText()
        {
            return "commands-migrate";
        }

        public override CommandPermission DefaultCommandPermission()
        {
            return new CommandPermission(false, PermissionLevel.Moderator, false, 5);
        }

        public override string PermissionFileSource()
        {
            return Constants.CUSTOM_COMMAND_FILE;
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            //check to see if it exists
            if (!Context.Instance.Tasks.ContainsKey(Constants.TRIGGER_CHAR + args[1]))
            {
                return $"The command {args[1]} does not exist on this server";
            }

            var task = Context.Instance.Tasks[Constants.TRIGGER_CHAR + args[1]];

            if (!(task is CustomTask))
            {
                return $"The command {args[1]} is not a custom command";
            }

            //check if the channel can be found, we require the hashtag
            var channelString = args[2].Substring(1);

            if (!(messageSource.Server.TextChannels.Count(s => s.Name == channelString) > 0))
            {
                return $"The channel {args[2]} cannot be found, did not forget the # before the channnel name?";
            }

            var targetChannel = messageSource.Server.TextChannels.First(s => s.Name == channelString);

            var customTask = (CustomTask)task;

            //get the current task form

            var form = customTask.GetCustomCommands().First(s => s.Channel == messageSource.Channel.Id);

            var newForm = new CustomCommandForm(form.CommandName, form.MaxArgs, targetChannel.Id, form.PermissionLevel, form.ReturnValues, form.CoolDown);

            customTask.AddContent(newForm);

            return $"Successfully copied command {args[1]} to channel {channelString}";
        }

        public override string UsageText()
        {
            return "(command) (channel name)";
        }
    }
}
