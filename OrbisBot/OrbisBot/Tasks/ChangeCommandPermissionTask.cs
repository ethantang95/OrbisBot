using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;

namespace OrbisBot.Tasks
{
    class ChangeCommandPermissionTask : TaskAbstract
    {
        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            if (args.Length != 3)
            {
                return new StringBuilder().AppendLine($"{Constants.SYNTAX_INTRO} !<command's name> <permission level>")
                    .AppendLine("Where as permission levels are: Owner, Admin, Moderator, User, RestrictedUser").ToString();
            }

            //first, check to see if the command exists
            if (!Context.Instance.Tasks.ContainsKey(args[1]))
            {
                return $"The command {args[1]} does not exist, did you mis-spell it? Type \"!Commands for a list of commands\"";
            }

            var commandToChange = Context.Instance.Tasks[args[1]];
            var userPermissionLevel = Context.Instance.ChannelPermission.GetUserPermission(messageSource.Channel.Id, messageSource.User.Id);
            PermissionLevel targetNewPermissionLevel;
            try
            {
                targetNewPermissionLevel = PermissionEnumMethods.ParseString(args[2], true);
            }
            catch (Exception e)
            {
                return "The rank you have input is not recognized, the ranks are: Owner, Admin, Moderator, User, RestrictedUser";
            }

            //if the user's level is below the command's level
            if (commandToChange.GetCommandPermission(messageSource.Channel.Id) > userPermissionLevel)
            {
                return "You do not have sufficient privledge to change the permission level of this command";
            }

            //if the user tries to set it above his permission level
            if (targetNewPermissionLevel > userPermissionLevel)
            {
                return "You cannot set the permission level of this command above your current permission level";
            }
            
            commandToChange.SetPermission(messageSource.Channel.Id, targetNewPermissionLevel);
            return $"Permission level for command {args[1]} has successfully been set to {targetNewPermissionLevel}";
        }

        public override string PermissionFileSource()
        {
            return Constants.CHANGE_COMMAND_PERMISSION_FILE;
        }

        public override CommandPermission DefaultCommands()
        {
            return new CommandPermission(false, PermissionLevel.Admin, true);
        }

        public override string CommandText()
        {
            return "!ChangeCommandPermission";
        }

        public override string AboutText()
        {
            return "Changes a command's minimum required permission level";
        }
    }
}
