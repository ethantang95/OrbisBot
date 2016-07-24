using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrbisBot.Permission;
using OrbisBot.TaskPermissions.Interfaces;

namespace OrbisBot.TaskPermissions.PermissionBuilders
{
    class RegisteredChannelTaskPermissionBuilder<T> : 
        TaskPermissionBuilderAbstract<RegisteredChannelTaskPermission<T>, RegisteredChannelTaskPermissionBuilder<T>>
        where T : ICommandPermissionForm
    {
        private ICollection<T> _permissions;

        private IPermissionSaver _saver;

        public RegisteredChannelTaskPermissionBuilder<T> SetPermissions(ICollection<T> permissions)
        {
            _permissions = permissions;
            return this;
        }

        public RegisteredChannelTaskPermissionBuilder<T> SetSaver(IPermissionSaver saver)
        {
            _saver = saver;
            return this;
        }

        public override RegisteredChannelTaskPermission<T> ConstructTaskTypePermissions(CommandPermission permission)
        {
            if (_permissions == null)
            {
                throw new ArgumentNullException("Permissions must be set for registered channel task permissions");
            }

            if (_saver == null)
            {
                throw new ArgumentNullException("There must be a saver for a registered channel task permission");
            }

            foreach (var permissionForm in _permissions)
            {
                permission.AddPermission(permissionForm);
            }
            return new RegisteredChannelTaskPermission<T>(permission, _permissions, _saver);
        }
    }
}
