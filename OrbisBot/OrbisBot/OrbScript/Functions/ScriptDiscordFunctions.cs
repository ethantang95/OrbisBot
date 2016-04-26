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
            var user = UserFinderUtil.FindUserMention(searchList, name, ignoreList);
            return user == null ? user : string.Empty;
        }

        public static string FindUser(string name, IEnumerable<User> searchList, HashSet<ulong> ignoreList)
        {
            var user = UserFinderUtil.FindUser(searchList, name, ignoreList)?.Name;
            return user == null ? user : string.Empty;
        }

        public static string MentionEveryone(IEnumerable<Role> roles)
        {
            //I am not sure if this can be blank... because it should not be
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
