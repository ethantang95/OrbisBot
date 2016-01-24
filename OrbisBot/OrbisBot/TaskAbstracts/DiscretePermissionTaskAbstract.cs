using OrbisBot.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;

namespace OrbisBot.TaskAbstracts
{
    //for tasks that are based on a certain permission only
    abstract class DiscretePermissionTaskAbstract : TaskAbstract
    {
        public override bool AllowTaskExecution(MessageEventArgs eventArgs)
        {
            var userPermission = GetUserPermission(eventArgs);

            return userPermission >= _commandPermission.DefaultLevel;
        }

        public override PermissionLevel GetCommandPermissionForChannel(long channelId)
        {
            return _commandPermission.DefaultLevel;
        }

        public override void SetCommandPermissionForChannel(long channelId, PermissionLevel newPermissionLevel)
        {
            throw new NotSupportedException("The permission level of this task cannot be changed");
        }

        public override void SetCoolDownForChannel(long channelId, int cooldown)
        {
            throw new NotSupportedException("The cooldown of this task cannot be changed");
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
    }
}
