﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.Permission
{
    public enum PermissionLevel { UsageDenied = 999, Developer = 6, Owner = 5, Admin = 4, Moderator = 3, User = 2, RestrictedUser = 1, Restricted = 0}
    //restricted should be absolutely restricted, nothing should allow restricted
    //usageDenied is reserved for tasks that are denied to be executed due to permission... a bit hacky

    public class PermissionEnumMethods
    {
        public static PermissionLevel ParseString(string toParse)
        {
            return ParseString(toParse, false);
        }

        public static PermissionLevel ParseString(string toParse, bool throwOnFail)
        {
            try
            {
                return (PermissionLevel)Enum.Parse(typeof(PermissionLevel), toParse);
            }
            catch (Exception e)
            {
                if (throwOnFail)
                {
                    throw e;
                }
                return PermissionLevel.User;
            }
        }
    }

    //here, it contains the permission level of the command and its associated server. There will be a default value which is set remotely or something if the server name does not exist in here
    class CommandPermission
    {
        public bool Disabled { get; set; }
        public bool OverrideMuting { get; private set; }
        public int DefaultCoolDown { get; private set; }
        public PermissionLevel DefaultLevel { get; set; }
        public Dictionary<long, ChannelPermissionSetting> ChannelPermissionLevel { get; set; }

        public CommandPermission(bool disabled, PermissionLevel defaultLevel, bool overrideMuting, int defaultCoolDown)
        {
            Disabled = disabled;
            DefaultLevel = defaultLevel;
            OverrideMuting = overrideMuting;
            DefaultCoolDown = defaultCoolDown;
            ChannelPermissionLevel = new Dictionary<long, ChannelPermissionSetting>();
        }
    }

    class ChannelPermissionSetting
    {
        public PermissionLevel PermissionLevel { get; set; }
        public int CoolDown { get; set; }

        public ChannelPermissionSetting(PermissionLevel permissionLevel, int coolDown)
        {
            PermissionLevel = permissionLevel;
            CoolDown = coolDown;
        }
    }
}
