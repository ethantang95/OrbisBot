using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace OrbisBot.TaskHelpers.UserFinder
{
    class UserFinderUtil
    {
        public static string FindUser(IEnumerable<User> users, string username)
        {
            //see if it's already a mention string
            if (username[0] == '<' && username[1] == '@' && username[username.Length - 1] == '>')
            {
                return username;
            }
            if (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(username))
            {
                return username;
            }
            //first, find by direct match
            var userToReturn = users.FirstOrDefault(s => s.Name == username);

            if (userToReturn != null)
            {
                return GetMentionString(userToReturn);
            }

            //if it is null
            userToReturn = users.FirstOrDefault(s => s.Name.ToLowerInvariant().Contains(username.ToLowerInvariant()));

            if (userToReturn != null)
            {
                return GetMentionString(userToReturn);
            }

            return username; //cannot find the user
        }

        public static string GetMentionString(User user)
        {
            return Mention.User(user);
        }
    }
}
