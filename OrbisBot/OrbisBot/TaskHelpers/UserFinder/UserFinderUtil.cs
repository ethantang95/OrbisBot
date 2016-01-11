﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace OrbisBot.TaskHelpers.UserFinder
{
    class UserFinderUtil
    {
        public static string FindUserMention(IEnumerable<User> users, string username)
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

            var user = FindUser(users, username);

            return user == null ? username : Mention.User(user);
        }
        public static User FindUser(IEnumerable<User> users, string username)
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

            //first, find by direct match
            var userToReturn = users.FirstOrDefault(s => s.Name == username);

            if (userToReturn != null)
            {
                return userToReturn;
            }

            //if it is null, we will deploy fuzzy search
            var candidates = users.Where(s => s.Name.ToLowerInvariant().Contains(username.ToLowerInvariant()));

            var sortedList = candidates.Select(s => new UserRank(s, s.Name.ToLowerInvariant().IndexOf(username.ToLowerInvariant()), s.Name.Length)).ToList();

            sortedList.Sort();

            if (sortedList.Count > 0)
            {
                userToReturn = sortedList.First().User;
            }

            if (userToReturn != null)
            {
                return userToReturn;
            }

            userToReturn = users.FirstOrDefault(s => s.Id.ToString() == username);

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

        public UserRank(User user, int searchPosition, int nameSize)
        {
            this.User = user;
            this.SearchPosition = searchPosition;
            this.NameSize = nameSize;
        }

        public int CompareTo(UserRank other)
        {
            return this.SearchPosition == other.SearchPosition ? this.NameSize.CompareTo(other.NameSize) : this.SearchPosition.CompareTo(other.SearchPosition);
        }
    }
}