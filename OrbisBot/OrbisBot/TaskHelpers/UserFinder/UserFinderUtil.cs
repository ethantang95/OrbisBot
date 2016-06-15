using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace OrbisBot.TaskHelpers.UserFinder
{
    static class UserFinderUtil
    {
        public static string FindUserMention(IEnumerable<User> users, string username, HashSet<ulong> hiddenUsers)
        {
            //see if it's already a mention string
            if (IsMentionFormat(username))
            {
                return username;
            }
            if (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(username))
            {
                return username;
            }

            var user = FindUser(users, username, hiddenUsers);

            return user == null ? username : user.Mention;
        }
        public static User FindUser(IEnumerable<User> users, string username, HashSet<ulong> hiddenUsers)
        {
            if (username[0] == '@')
            {
                //strip the mention
                username = username.Substring(1);
            }

            if (IsMentionFormat(username))
            {
                username = username.Substring(2, username.Length - 3);
            }

            var usersFiltered = users.Where(s => !hiddenUsers.Contains(s.Id));

            //first, find by direct match
            var userToReturn = usersFiltered.FirstOrDefault(s => s.Name == username);

            if (userToReturn != null)
            {
                return userToReturn;
            }

            //then try by id
            userToReturn = usersFiltered.FirstOrDefault(s => s.Id.ToString() == username);

            if (userToReturn != null)
            {
                return userToReturn;
            }

            //by discriminator
            userToReturn = usersFiltered.FirstOrDefault(s => s.Discriminator.ToString() == username);

            if (userToReturn != null)
            {
                return userToReturn;
            }

            //if it is null, we will deploy fuzzy search
            var candidates = usersFiltered.Where(s => s.Name.ToLowerInvariant().Contains(username.ToLowerInvariant()));

            var sortedList = candidates.Select(s => new UserRank(s, s.Name.ToLowerInvariant().IndexOf(username.ToLowerInvariant()), s.Name.Length,
                s.Name.Length - s.Name.ToLowerInvariant().IndexOf(username.ToLowerInvariant()) == username.Length)) //to determine if it matches at the back
                .ToList();

            sortedList.Sort();

            if (sortedList.Count > 0)
            {
                userToReturn = sortedList.First().User;
            }

            if (userToReturn != null)
            {
                return userToReturn;
            }

            return null; //cannot find the user
        }

        private static bool IsMentionFormat(string username)
        {
            return username[0] == '<' && username[1] == '@' && username[username.Length - 1] == '>';
        }
    }

    class UserRank : IComparable<UserRank>
    {
        public User User { get; private set; }
        public int SearchPosition { get; private set; }
        public int NameSize { get; private set; }
        public bool BackMatch { get; private set; }

        public UserRank(User user, int searchPosition, int nameSize, bool backMatch)
        {
            this.User = user;
            this.SearchPosition = searchPosition;
            this.NameSize = nameSize;
            this.BackMatch = backMatch;
        }

        public int CompareTo(UserRank other)
        {
            if (this.SearchPosition == 0 && other.SearchPosition == 0) //first match rules all
            {
                return this.NameSize.CompareTo(other.NameSize);
            }
            else if (this.BackMatch && other.BackMatch) //if both have a back match
            {
                return this.NameSize.CompareTo(other.NameSize);
            }
            else if (this.BackMatch || other.BackMatch)
            {
                return other.BackMatch.CompareTo(this.BackMatch);//a matching in the back will preceed size comparison
            }
            return this.SearchPosition == other.SearchPosition ?
                    this.NameSize.CompareTo(other.NameSize) :
                    this.SearchPosition.CompareTo(other.SearchPosition);
        }
    }
}
