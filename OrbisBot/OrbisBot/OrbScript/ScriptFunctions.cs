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
    }
}
