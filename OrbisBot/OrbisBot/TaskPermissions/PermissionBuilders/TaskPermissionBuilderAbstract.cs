using OrbisBot.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.TaskPermissions.PermissionBuilders
{

    //a task permission builder will populate the initial fields inside a CommandPermission object

    //we will return as a final result, a task permission object, but with regards to the curiousor pattern
    //we will have as a parameter to the parent type, the child type
    abstract class TaskPermissionBuilderAbstract<T, W>
        where T : TaskPermissionAbstract
        where W : TaskPermissionBuilderAbstract<T, W>
    {
        private bool _disabled = false;
        private bool _overrideMuting = false;
        private int _defaultCoolDown = 30;
        private PermissionLevel _defaultLevel = PermissionLevel.User;

        public W SetDisabled(bool value)
        {
            _disabled = value;
            return (W)this;
        }

        public W SetOverrideMuting(bool value)
        {
            _overrideMuting = value;
            return (W)this;
        }

        public W SetDefaultCooldown(int value)
        {
            _defaultCoolDown = value;
            return (W)this;
        }

        public W SetDefaultLevel(PermissionLevel value)
        {
            _defaultLevel = value;
            return (W)this;
        }

        public T BuildPermission()
        {
            var permission = new CommandPermission(_disabled, _defaultLevel, _overrideMuting, _defaultCoolDown);
            return ConstructTaskTypePermissions(permission);
        }
        public abstract T ConstructTaskTypePermissions(CommandPermission permission);
    }
}
