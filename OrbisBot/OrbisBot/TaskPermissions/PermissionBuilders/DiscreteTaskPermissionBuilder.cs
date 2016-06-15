using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrbisBot.Permission;

namespace OrbisBot.TaskPermissions.PermissionBuilders
{
    class DiscreteTaskPermissionBuilder : TaskPermissionBuilderAbstract<DiscreteTaskPermission, DiscreteTaskPermissionBuilder>
    {
        public override DiscreteTaskPermission ConstructTaskTypePermissions(CommandPermission permission)
        {
            return new DiscreteTaskPermission(permission);
        }
    }
}
