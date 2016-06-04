using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;

namespace OrbisBot.TaskPermissions
{
    class DiscreteTaskPermission : TaskPermissionAbstract
    {
        public DiscreteTaskPermission(CommandPermission permission) : base(permission)
        {

        }

        public override bool AllowTaskExecution(MessageEventArgs eventArgs)
        {
            var userPermission = GetUserPermission(eventArgs);

            return userPermission >= CommandPermission.DefaultLevel;
        }

        public override PermissionLevel GetCommandPermissionForChannel(ulong channelId)
        {
            return CommandPermission.DefaultLevel;
        }

        public override void SetCommandPermissionForChannel(ulong channelId, PermissionLevel newPermissionLevel)
        {
            throw new NotSupportedException("The permission level of this task cannot be changed");
        }

        public override void SetCoolDownForChannel(ulong channelId, int cooldown)
        {
            throw new NotSupportedException("The cooldown of this task cannot be changed");
        }

        private PermissionLevel GetUserPermission(MessageEventArgs messageEventArgs)
        {
            return Context.Instance.ChannelPermission.GetUserPermission(messageEventArgs.Channel.Id,
                messageEventArgs.User.Id);
        }
    }
}
