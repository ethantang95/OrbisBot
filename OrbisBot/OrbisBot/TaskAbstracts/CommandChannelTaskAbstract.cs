using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using System.Configuration;
using OrbisBot.TaskHelpers.AdminUtils;
using OrbisBot.TaskPermissions;

namespace OrbisBot.TaskAbstracts
{
    abstract class CommandChannelTaskAbstract: TaskAbstract
    {
        public CommandChannelTaskAbstract(TaskPermissionAbstract permission) : base(permission)
        {

        }

        public override string ExceptionMessage(Exception ex, MessageEventArgs eventArgs)
        {
            return $"Message crashed with the exception {ex.ToString()}";
        }
    }
}
