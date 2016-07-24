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
        public void SaveSettings<T>(ICollection<T> permission) where T : ICommandPermissionForm
        {
            if (!(permission is List<CustomCommandForm>))
            {
                throw new ArgumentException("What is passed into save custom commands is not of a type of custom command form");
            }

            //we have nothing to save, just return
            if (permission.Count == 0)
            {
                return;
            }

            CustomCommandFileHandler.SaveCustomTask((List<CustomCommandForm>)permission);
        }
    }
}
