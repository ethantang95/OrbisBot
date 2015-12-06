using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.Permission
{
    public enum PermissionLevel { Owner = 5, Admin = 4, Moderator = 3, User = 2, RestrictedUser = 1}

    public class PermissionEnumMethods
    {
        public static PermissionLevel ParseString(string toParse)
        {
            return (PermissionLevel)Enum.Parse(typeof(PermissionLevel), toParse);
        }
    }

    //here, it contains the permission level of the command and its associated server. There will be a default value which is set remotely or something if the server name does not exist in here
    class CommandPermission
    {
        public bool Disabled { get; set; }
        public bool OverrideMuting { get; private set; }
        public PermissionLevel DefaultLevel { get; set; }
        public Dictionary<long, PermissionLevel> ChannelPermissionLevel { get; set; }

        public CommandPermission(bool disabled, PermissionLevel defaultLevel, bool overrideMuting)
        {
            Disabled = disabled;
            DefaultLevel = defaultLevel;
            OverrideMuting = overrideMuting;
            ChannelPermissionLevel = new Dictionary<long, PermissionLevel>();
        }

        public Dictionary<string, string> toFileOutput()
        {
            Dictionary<string, string> toReturn = new Dictionary<string, string>();
            toReturn.Add(Constants.COMMAND_DISABLED, Disabled.ToString());
            toReturn.Add(Constants.COMMAND_DEFAULT, DefaultLevel.ToString());
            toReturn.Add(Constants.COMMAND_OVERRIDE, OverrideMuting.ToString());
            ChannelPermissionLevel.ToList().ForEach(s => toReturn.Add(s.Key.ToString(), s.Value.ToString()));

            return toReturn;
        }
    }
}
