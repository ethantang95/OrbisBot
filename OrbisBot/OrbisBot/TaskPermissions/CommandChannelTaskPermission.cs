using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using OrbisBot.TaskHelpers.AdminUtils;

namespace OrbisBot.TaskPermissions
{
    class CommandChannelTaskPermission : TaskPermissionAbstract
    {
        public CommandChannelTaskPermission(CommandPermission permission) : base(permission)
        {

        }

        public override bool AllowTaskExecution(MessageEventArgs eventArgs)
        {
            return GeneralAdminUtils.IsCommandChannel(eventArgs.Channel.Id) && Context.Instance.ChannelPermission.IsDeveloper(eventArgs.Channel.Id, eventArgs.User.Id);
        }

        public override PermissionLevel GetCommandPermissionForChannel(ulong channelId)
        {
            if (GeneralAdminUtils.IsCommandChannel(channelId))
            {
                return PermissionLevel.Developer;
            }
            else
            {
                return PermissionLevel.Restricted;
            }
        }

        public override void SetCommandPermissionForChannel(ulong channelId, PermissionLevel newPermissionLevel)
        {
            throw new UnauthorizedAccessException("This should not be called for command tasks");
        }

        public override void SetCoolDownForChannel(ulong channelId, int cooldown)
        {
            throw new UnauthorizedAccessException("This should not be called for command tasks");
        }
    }
}
