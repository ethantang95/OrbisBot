using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrbisBot.Permission;

namespace OrbisBot.TaskPermissions.PermissionBuilders
{
    class CommandChannelTaskPermissionBuilder : TaskPermissionBuilderAbstract<CommandChannelTaskPermission, CommandChannelTaskPermissionBuilder>
    {
        public override CommandChannelTaskPermission ConstructTaskTypePermissions(CommandPermission permission)
        {
            //this will override the default ones
            var newPermission = new CommandPermission(false, PermissionLevel.Developer, true, 1);

            return new CommandChannelTaskPermission(newPermission);
        }
    }
}
