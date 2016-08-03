using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.OrbScript
{
    enum OrbScriptBuildType { Standard, Events, JoinLeave, PrivateMessage};
    class OrbScriptConfiger
    {
        public IEnumerable<User> UserList { get; private set; }
        public IEnumerable<Role> RoleList { get; private set; }
        public User FocusUser { get; private set; }
        public HashSet<ulong> IgnoreList { get; private set; }
        public OrbScriptBuildType BuildType { get; private set; }
        public MessageEventArgs EventArgs { get; private set; }
        public string SourceCommand { get; private set; }
        public int Iterations { get; private set; }

        public OrbScriptConfiger(User focusUSer, OrbScriptBuildType buildType = OrbScriptBuildType.Standard)
        {
            FocusUser = focusUSer;
            BuildType = buildType;
            UserList = new List<User>();
            RoleList = new List<Role>();
            IgnoreList = new HashSet<ulong>();
            SourceCommand = string.Empty;
        }

        public OrbScriptConfiger SetUserList(IEnumerable<User> users)
        {
            UserList = users;
            return this;
        }

        public OrbScriptConfiger SetRoleList(IEnumerable<Role> roles)
        {
            RoleList = roles;
            return this;
        }

        public OrbScriptConfiger SetIgnoreList(HashSet<ulong> list)
        {
            IgnoreList = list;
            return this;
        }

        public OrbScriptConfiger SetEventArgs(MessageEventArgs eventArgs)
        {
            EventArgs = eventArgs;
            return this;
        }

        public OrbScriptConfiger SetSourceCommand(string command)
        {
            SourceCommand = command;
            return this;
        }

        public OrbScriptConfiger SetCallIterations(int iterations)
        {
            Iterations = iterations;
            return this;
        }
    }
}
