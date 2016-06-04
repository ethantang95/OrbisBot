using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.Permission
{
    interface ICommandPermissionForm
    {
        ulong Channel { get; set; }
        PermissionLevel PermissionLevel { get; set; }
        int CoolDown { get; set; }
    }
}
