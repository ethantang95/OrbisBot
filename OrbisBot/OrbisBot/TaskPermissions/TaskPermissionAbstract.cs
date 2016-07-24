using Discord;
using OrbisBot.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.TaskPermissions
{
    abstract class TaskPermissionAbstract
    {
        public CommandPermission CommandPermission { get; private set; }

        public TaskPermissionAbstract(CommandPermission permission)
        {
            CommandPermission = permission;
        }

        public abstract bool AllowTaskExecution(MessageEventArgs eventArgs);

        public abstract PermissionLevel GetCommandPermissionForChannel(ulong channelId);

        public abstract void SetCommandPermissionForChannel(ulong channelId, PermissionLevel newPermissionLevel);

        public abstract void SetCoolDownForChannel(ulong channelId, int cooldown);

        public virtual void AddPermission(ICommandPermissionForm permission)
        {
            CommandPermission.AddPermission(permission);
        }

        public virtual void RemovePermission(ulong channelId)
        {
            CommandPermission.ChannelPermission.Remove(channelId);
        }

        public virtual void UpdatePermission(ICommandPermissionForm permission)
        {
            //kind of a really lazy implementation to ensure update
            RemovePermission(permission.Channel);
            AddPermission(permission);
        }
    }
}
