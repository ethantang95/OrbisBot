using OrbisBot.TaskPermissions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrbisBot.Permission;
using OrbisBot.TaskHelpers.CustomCommands;

namespace OrbisBot.TaskPermissions.Implmentations
{
    class SaveCustomCommands : IPermissionSaver
    {
        public void SaveSettings(IEnumerable<ICommandPermissionForm> permission)
        {
            if (!(permission is List<CustomCommandForm>))
            {
                throw new ArgumentException("What is passed into save custom commands is not of a type of custom command form");
            }
            CustomCommandFileHandler.SaveCustomTask((List<CustomCommandForm>)permission);
        }
    }
}
