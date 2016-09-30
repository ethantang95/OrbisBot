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
    class ChangeCommandPermissionTask : TaskAbstract
    {
        public ChangeCommandPermissionTask(FileBasedTaskPermission permission) : base(permission)
        {

        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            //first, check to see if the command exists
            if (!Context.Instance.Tasks.ContainsKey(args[1]))
            {
                return $"The command {args[1]} does not exist, did you mis-spell it? Type \"-Commands for a list of commands\"";
            }

            var commandToChange = Context.Instance.Tasks[args[1]];
            var userPermissionLevel = Context.Instance.ChannelPermission.GetUserPermission(messageSource.Channel.Id, messageSource.User.Id);
            PermissionLevel targetNewPermissionLevel;
            try
            {
                targetNewPermissionLevel = EnumParser.ParseString(args[2], true, PermissionLevel.User);
            }
            catch (Exception e)
            {
                return "The rank you have input is not recognized, the ranks are: Owner, Admin, Moderator, User, RestrictedUser";
            }

            //if the user's level is below the command's level
            if (commandToChange.TaskPermission.GetCommandPermissionForChannel(messageSource.Channel.Id) > userPermissionLevel)
            {
                return "You do not have sufficient privledge to change the permission level of this command";
            }

            //if the user tries to set it above his permission level
            if (targetNewPermissionLevel > userPermissionLevel)
            {
                return "You cannot set the permission level of this command above your current permission level";
            }

            //if the user tries to change it to restricted
            if (targetNewPermissionLevel == PermissionLevel.Restricted)
            {
                return "You cannot set the permission level to restricted as it is reserved for special cases only";
            }
            
            commandToChange.TaskPermission.SetCommandPermissionForChannel(messageSource.Channel.Id, targetNewPermissionLevel);
            return $"Permission level for command {args[1]} has successfully been set to {targetNewPermissionLevel}";
        }

        public override string CommandText()
        {
            return "commands-permission";
        }

        public override string AboutText()
        {
            return "Changes a command's minimum required permission level";
        }

        public override string ExceptionMessage(Exception ex, MessageEventArgs eventArgs)
        {
            if (ex.GetType() == typeof(NotSupportedException))
            {
                return ex.Message;
            }
            return "An error has occurred when trying to change the command's permission, the developers has been notified of this problem";
        }

        public override bool CheckArgs(string[] args)
        {
            return args.Length == 3;
        }

        public override string UsageText()
        {
            return "(command) (role) \nThe ranks are Owner, Admin, Moderator, User, RestrictedUser";
        }
    }
}
