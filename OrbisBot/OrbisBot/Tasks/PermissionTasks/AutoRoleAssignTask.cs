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
    class AutoRoleAssignTask : TaskAbstract
    {
        public AutoRoleAssignTask(DiscreteTaskPermission permission) : base(permission)
        {
        }

        public override string AboutText()
        {
            return "Automatically assign the roles for this bot based on the permission given to the users in this channel. Type commit as a parameter to actually assign the roles, type server after commit as a parameter to set it up for the entire server";
        }

        public override bool CheckArgs(string[] args)
        {
            if (args.Length > 3)
            {
                return false;
            }
            else if (args.Length >= 2 && !args[1].Equals("commit", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }
            else if (args.Length == 3 && !args[2].Equals("server", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }
            return true;
        }

        public override string CommandText()
        {
            return "user-autopermission";
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            var users = messageSource.Channel.Users.ToList();

            var newRoles = new Dictionary<User, PermissionLevel>();

            users.ForEach(s => newRoles.Add(s, messageSource.Server.Owner.Id == s.Id ? PermissionLevel.Owner : DeterminePermissionLevelFromPermissions(s.ServerPermissions))  
            );

            newRoles = newRoles.Where(s => s.Value != PermissionLevel.User).ToDictionary(s => s.Key, s => s.Value);
            if (args.Length == 3)
            {
                var channels = messageSource.Server.TextChannels;
                foreach (var channel in channels)
                {
                    newRoles.ToList().ForEach(s => Context.Instance.ChannelPermission.SetUserPermission(messageSource.Server.Id, channel.Id, s.Key.Id, s.Value));
                }

                return "roles has been successfully automatically assigned";
            }
            else if (args.Length == 2)
            {
                newRoles.ToList().ForEach(s => Context.Instance.ChannelPermission.SetUserPermission(messageSource.Server.Id, messageSource.Channel.Id, s.Key.Id, s.Value));

                return "roles has been successfully automatically assigned";
            }
            else
            {
                var messageBuilder = new StringBuilder().AppendLine("The following roles will be assigned if you commit:");
                newRoles.ToList().ForEach(s => messageBuilder.AppendLine($"{s.Key.Name} => {s.Value}"));
                messageBuilder.AppendLine($"type \"{CommandText()} commit\" to assign these roles");
                return messageBuilder.ToString();
            }
        }

        public override string UsageText()
        {
            return "*<commit> *<server>";
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
