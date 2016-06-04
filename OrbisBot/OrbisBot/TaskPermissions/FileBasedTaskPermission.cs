using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;

namespace OrbisBot.TaskPermissions
{
    class FileBasedTaskPermission : TaskPermissionAbstract
    {
        private string _fileSource;
        public FileBasedTaskPermission(CommandPermission permission, string fileSource) : base(permission)
        {
            _fileSource = fileSource;
        }
        public override bool AllowTaskExecution(MessageEventArgs eventArgs)
        {
            //here, we check for permissions, first, check the server permissions
            //default permission is always user
            var userPermission = GetUserPermission(eventArgs);

            //get the command permission now
            var commandPermission = GetCommandPermissionForChannel(eventArgs.Channel.Id);

            if (commandPermission > userPermission || userPermission == PermissionLevel.Restricted)
            {
                return false; //the user does not have the rights to perform this task
            }

            return true;
        }

        public override PermissionLevel GetCommandPermissionForChannel(ulong channelId)
        {
            if (CommandPermission.ChannelPermission.ContainsKey(channelId))
            {
                return CommandPermission.ChannelPermission[channelId].PermissionLevel;
            }
            return CommandPermission.DefaultLevel;
        }

        public override void SetCommandPermissionForChannel(ulong channelId, PermissionLevel newPermissionLevel)
        {
            if (CommandPermission.ChannelPermission.ContainsKey(channelId))
            {
                CommandPermission.ChannelPermission[channelId].PermissionLevel = newPermissionLevel;
            }
            else
            {
                CommandPermission.ChannelPermission.Add(channelId, new ChannelPermissionSetting(newPermissionLevel, CommandPermission.DefaultCoolDown));
            }
            FileHelper.WriteObjectToFile(_fileSource, CommandPermission);
        }

        public override void SetCoolDownForChannel(ulong channelId, int cooldown)
        {
            if (CommandPermission.ChannelPermission.ContainsKey(channelId))
            {
                CommandPermission.ChannelPermission[channelId].CoolDown = cooldown;
            }
            else
            {
                CommandPermission.ChannelPermission.Add(channelId, new ChannelPermissionSetting(CommandPermission.DefaultLevel, cooldown));
            }
            FileHelper.WriteObjectToFile(_fileSource, CommandPermission);
        }

        private PermissionLevel GetUserPermission(MessageEventArgs messageEventArgs)
        {
            return Context.Instance.ChannelPermission.GetUserPermission(messageEventArgs.Channel.Id,
                messageEventArgs.User.Id);
        }
    }
}
