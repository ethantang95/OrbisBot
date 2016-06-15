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
    }
}
