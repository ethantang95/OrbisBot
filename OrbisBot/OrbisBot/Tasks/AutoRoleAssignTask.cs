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
    class AutoRoleAssignTask : DiscretePermissionTaskAbstract
    {
        public override string AboutText()
        {
            return "Automatically assign the roles for this bot based on the permission given to the users in this channel. Type commit as a parameter to actually assign the roles";
        }

        public override bool CheckArgs(string[] args)
        {
            if (args.Length > 2)
            {
                return false;
            }
            else if (args.Length == 2 && !args[1].Equals("commit", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }
            return true;
        }

        public override string CommandText()
        {
            return "user-autopermission";
        }

        public override CommandPermission DefaultCommandPermission()
        {
            return new CommandPermission(false, PermissionLevel.Owner, true, 1);
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            var users = messageSource.Channel.Members.ToList();

            var newRoles = new Dictionary<User, PermissionLevel>();

            users.ForEach(s => newRoles.Add(s, messageSource.Server.Owner.Id == s.Id ? PermissionLevel.Owner : DeterminePermissionLevelFromPermissions(s.GetServerPermissions()))  
            );

            newRoles = newRoles.Where(s => s.Value != PermissionLevel.User).ToDictionary(s => s.Key, s => s.Value);

            if (args.Length == 2)
            {
                newRoles.ToList().ForEach(s => Context.Instance.ChannelPermission.SetUserPermission(messageSource.Server.Id, messageSource.Channel.Id, s.Key.Id, s.Value));

                return "roles has been successfully automatically assigned";
            }
            else
            {
                var messageBuilder = new StringBuilder().AppendLine("The following roles will be assigned if you commit:");
                newRoles.ToList().ForEach(s => messageBuilder.AppendLine($"{s.Key.Name} => {s.Value}"));
                messageBuilder.AppendLine("type \"-AutoRole commit\" to assign these roles");
                return messageBuilder.ToString();
            }
        }

        public override string UsageText()
        {
            return "OPTIONAL<commit>";
        }

        private PermissionLevel DeterminePermissionLevelFromPermissions(ServerPermissions permissions)
        {
            //the general rule of thumb is a mod can change the state of an user and its belongings while an admin can change the state of the entire server
            //however, sometimes some servers will have mixed roles for who knows what reason, I'll set the arbitary threshold to be at least 3 admin level commands to make a person admin

            var adminPermissionsCount = 0;
            var modPermissionsCount = 0;

            //mod permissions
            if (permissions.BanMembers)
            {
                modPermissionsCount += 3;
            }
            if (permissions.KickMembers)
            {
                modPermissionsCount += 3;
            }
            if (permissions.DeafenMembers)
            {
                modPermissionsCount++;
            }
            if (permissions.ManageMessages)
            {
                modPermissionsCount++;
            }
            if (permissions.MoveMembers)
            {
                modPermissionsCount++;
            }
            if (permissions.MuteMembers)
            {
                modPermissionsCount++;
            }

            if (permissions.ManageChannels)
            {
                adminPermissionsCount++;
            }
            if (permissions.ManageRoles)
            {
                adminPermissionsCount += 3;
            }
            if (permissions.ManageServer)
            {
                adminPermissionsCount += 3;
            }

            if (adminPermissionsCount >= 3)
            {
                return PermissionLevel.Admin;
            }
            else if (adminPermissionsCount > 0 || modPermissionsCount >= 3)
            {
                return PermissionLevel.Moderator;
            }
            else
            {
                return PermissionLevel.User;
            }

        }
    }
}
