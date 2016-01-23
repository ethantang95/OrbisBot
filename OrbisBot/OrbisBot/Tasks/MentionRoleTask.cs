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
    class MentionRoleTask : FilePermissionTaskAbstract
    {
        public override string AboutText()
        {
            return "Sends a notification with a message to everyone with the given role";
        }

        public override bool CheckArgs(string[] args)
        {
            return args.Length == 2 || args.Length == 3;
        }

        public override string CommandText()
        {
            return "role-mention";
        }

        public override CommandPermission DefaultCommandPermission()
        {
            return new CommandPermission(false, PermissionLevel.Moderator, false, 1);
        }

        public override string PermissionFileSource()
        {
            return Constants.ROLE_MENTION_FILE;
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            if (args.Length < 2 || args.Length > 3)
            {
                return $"{Constants.USAGE_INTRO} \"<role name>\" \"<OPTIONAL (message)>\"";
            }
            var role = messageSource.Server.Roles.FirstOrDefault(s => args[1].Equals(s.Name, StringComparison.InvariantCultureIgnoreCase));

            if (role == null)
            {
                return $"The role *{args[1]}* cannot be found";
            }
            else if (role.Members.ToList().Count > 40)
            {
                return $"There are too many people that has this role, a mention of over 40 people will cause too many disruption, please try and create smaller roles";
            }
            else if (role.IsEveryone)
            {
                return $"This command is filtered from an everyone mention";
            }

            var returnMessage = new StringBuilder();

            role.Members.ToList().ForEach(s => returnMessage.Append(Mention.User(s) + " " ));

            if (args.Length == 3)
            {
                returnMessage.AppendLine(args[2]);
            }

            return returnMessage.ToString();
            
        }

        public override string UsageText()
        {
            return "(\"role\") OPTIONAL(\"message\")";
        }
    }
}
