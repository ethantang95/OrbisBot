using OrbisBot.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace OrbisBot.TaskAbstracts
{ 

    //this class exists for tasks that belongs to a registered channel
    abstract class RegisteredChannelTaskAbstract : TaskAbstract
    {
        public override PermissionLevel GetCommandPermissionForChannel(ulong channelId)
        {
            //basically, restrict anything that is not on the permission list
            if (_commandPermission.ChannelPermission.ContainsKey(channelId))
            {
                return _commandPermission.ChannelPermission[channelId].PermissionLevel;
            }
            return PermissionLevel.UsageDenied;
        }

        public override void SetCommandPermissionForChannel(ulong channelId, PermissionLevel newPermissionLevel)
        {
            if (!_commandPermission.ChannelPermission.ContainsKey(channelId))
            {
                throw new UnauthorizedAccessException("You cannot change the permission for this command as your channel do not have access to this command");
            }
            _commandPermission.ChannelPermission[channelId].PermissionLevel = newPermissionLevel;
            SaveSettings(_commandPermission);
        }

        public override void SetCoolDownForChannel(ulong channelId, int cooldown)
        {
            if (!_commandPermission.ChannelPermission.ContainsKey(channelId))
            {
                throw new UnauthorizedAccessException("You cannot change the permission for this command as your channel do not have access to this command");
            }
            _commandPermission.ChannelPermission[channelId].CoolDown = cooldown;
            SaveSettings(_commandPermission);
        }

        public override bool AllowTaskExecution(MessageEventArgs messageEventArgs)
        {
            //here, we check for permissions, first, check the server permissions
            //default permission is always user
            var userPermission = GetUserPermission(messageEventArgs);

            //get the command permission now
            var commandPermission = GetCommandPermissionForChannel(messageEventArgs.Channel.Id);

            if (commandPermission > userPermission || userPermission == PermissionLevel.Restricted)
            {
                return false; //the user does not have the rights to perform this task
            }

            return true;
        }

        private PermissionLevel GetUserPermission(MessageEventArgs messageEventArgs)
        {
            return Context.Instance.ChannelPermission.GetUserPermission(messageEventArgs.Channel.Id,
                messageEventArgs.User.Id);
        }

        public override string ExceptionMessage(Exception ex, MessageEventArgs eventArgs)
        {
            return String.Empty;
        }

        public abstract void SaveSettings(CommandPermission commandPermission);
    }
}
