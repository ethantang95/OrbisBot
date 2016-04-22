using OrbisBot.TaskAbstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;

namespace OrbisBot.Tasks
{
    class ChangeCoolDownTask : FilePermissionTaskAbstract
    {
        public override string AboutText()
        {
            return "Changes the cooldown of a task";
        }

        public override bool CheckArgs(string[] args)
        {
            var a = 0;
            return (args.Length == 3 && Int32.TryParse(args[2], out a));
        }

        public override string CommandText()
        {
            return "commands-cooldown";
        }

        public override CommandPermission DefaultCommandPermission()
        {
            return new CommandPermission(false, PermissionLevel.Moderator, false, 1);
        }

        public override string PermissionFileSource()
        {
            return Constants.CHANGE_COOL_DOWN_FILE;
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            //first, see if the command exists
            var commandText = Constants.TRIGGER_CHAR + args[1];
            if (!Context.Instance.Tasks.ContainsKey(commandText))
            {
                return "The command does not exist";
            }

            var command = Context.Instance.Tasks[commandText];

            //now, get the permission of the command
            var commandPermission = command.GetCommandPermissionForChannel(messageSource.Channel.Id);

            var userPermission = Context.Instance.ChannelPermission.GetUserPermission(messageSource.Channel.Id, messageSource.User.Id);

            if (commandPermission > userPermission)
            {
                return "You do not have sufficient permission to change the cooldown of the command";
            }

            var cooldown = Int32.Parse(args[2]);

            command.SetCoolDownForChannel(messageSource.Channel.Id, cooldown);

            return "Cooldown successfully set for command";
        }

        public override string UsageText()
        {
            return "(command) (cooldown in seconds)";
        }
    }
}
