﻿using OrbisBot.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.TaskPermissions.Interfaces
{
    interface IPermissionSaver
    {
        void SaveSettings(IEnumerable<ICommandPermissionForm> permission);
    }
}
