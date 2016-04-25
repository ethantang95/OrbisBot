using Discord;
using OrbisBot.TaskHelpers.UserFinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.OrbScript
{
    static class ScriptFunctions
    {
        public static string UserMention(string name, IEnumerable<User> searchList, HashSet<ulong> ignoreList)
        {
            return UserFinderUtil.FindUserMention(searchList, name, ignoreList);
        }

        public static string FindUser(string name, IEnumerable<User> searchList, HashSet<ulong> ignoreList)
        {
            return UserFinderUtil.FindUser(searchList, name, ignoreList).Name;
        }

        public static string MentionEveryone(IEnumerable<Role> roles)
        {
            return roles.First(s => s.IsEveryone).Mention;
        }

        public static string MentionGroup(IEnumerable<User> users)
        {
            var builder = new StringBuilder();

            foreach (var user in users)
            {
                builder.Append(UserMention(user.Name, users, new HashSet<ulong>()));
            }

            return builder.ToString();
        }
    }
}
