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
    class MentionRoleTask : TaskAbstract
    {
        public MentionRoleTask(FileBasedTaskPermission permission) : base(permission)
        {

        }

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

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
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

            role.Members.ToList().ForEach(s => returnMessage.Append(s.Mention + " " ));

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
