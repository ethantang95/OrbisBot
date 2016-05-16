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
        protected CommandPermission _commandPermission;
        private Dictionary<ulong, DateTime> _lastUsed;

        public TaskPermissionAbstract(CommandPermission permission)
        {
            _commandPermission = permission;
            _lastUsed = new Dictionary<ulong, DateTime>();
            _commandPermission.ChannelPermission.Keys.ToList().ForEach(s => _lastUsed.Add(s, new DateTime(0)));
        }

        public abstract bool AllowTaskExecution(MessageEventArgs eventArgs);

        public abstract PermissionLevel GetCommandPermissionForChannel(ulong channelId);

        public abstract void SetCommandPermissionForChannel(ulong channelId, PermissionLevel newPermissionLevel);

        public abstract void SetCoolDownForChannel(ulong channelId, int cooldown);
    }
}
