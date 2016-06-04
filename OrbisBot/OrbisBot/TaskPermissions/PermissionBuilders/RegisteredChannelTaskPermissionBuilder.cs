using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrbisBot.Permission;
using OrbisBot.TaskPermissions.Interfaces;

namespace OrbisBot.TaskPermissions.PermissionBuilders
{
    class RegisteredChannelTaskPermissionBuilder : TaskPermissionBuilderAbstract<RegisteredChannelTaskPermission, RegisteredChannelTaskPermissionBuilder>
    {
        private IEnumerable<ICommandPermissionForm> _permissions;

        private IPermissionSaver _saver;

        public RegisteredChannelTaskPermissionBuilder SetPermissions(IEnumerable<ICommandPermissionForm> permissions)
        {
            _permissions = permissions;
            return this;
        }

        public RegisteredChannelTaskPermissionBuilder SetSaver(IPermissionSaver saver)
        {
            _saver = saver;
            return this;
        }

        public override RegisteredChannelTaskPermission ConstructTaskTypePermissions(CommandPermission permission)
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
            return new RegisteredChannelTaskPermission(permission, _permissions, _saver);
        }
    }
}
