using OrbisBot.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.TaskHelpers.CustomCommands
{
    class CustomCommandForm : ICommandPermissionForm
    {
        public string CommandName { get; set; }
        public int MaxArgs { get; set; }
        public ulong Channel { get; set; }
        public PermissionLevel PermissionLevel { get; set; }
        public List<string> ReturnValues { get; set; }
        public int CoolDown { get; set; }

        public CustomCommandForm(string commandName, int maxArgs, ulong channel, PermissionLevel permissionLevel, List<string> returnValues, int coolDown)
        {
            CommandName = commandName;
            MaxArgs = maxArgs;
            Channel = channel;
            PermissionLevel = permissionLevel;
            ReturnValues = returnValues;
            CoolDown = coolDown;
        }
    }
}
