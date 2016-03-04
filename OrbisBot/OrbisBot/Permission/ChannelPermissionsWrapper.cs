using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.Permission
{
    class ChannelPermissionsWrapper
    {
        public Dictionary<ulong, ChannelPermission> ChannelPermissions { get; private set; }

        //the format I will have the file is one master file that contains the id of all the channels
        //then the channels file will be name by the ID of that
        public ChannelPermissionsWrapper()
        {
            ChannelPermissions = new Dictionary<ulong, ChannelPermission>();
            GetAllRegisteredChannel();
        }

        private void GetAllRegisteredChannel()
        {
            var channels = FileHelper.GetContentFromFile(Constants.REGISTERED_CHANNEL_FILE);
            if (channels != null)
            {
                channels.ForEach(s => SetChannelPermission(UInt64.Parse(s)));
            }
        }

        private void SetChannelPermission(ulong channelId)
        {
            var channelSettings = FileHelper.GetObjectFromFile<ChannelPermission>(Path.Combine(Constants.CHANNELS_OPTIONS_FOLDER, channelId.ToString()));

            //The file will contain the channel ID, then the server ID, then is it muted or not
            //Rest will be individual's permission levels

            ChannelPermissions.Add(channelId, channelSettings);
        }

        private void CheckAndCreateChannel(ulong serverId, ulong channelId)
        {
            if (!ChannelPermissions.ContainsKey(channelId))
            {
                //find other channels under the same server with a main channel id
                var mainChannel = Context.Instance.ChannelPermission.ChannelPermissions?.FirstOrDefault(s => s.Value.ServerId == serverId);

                ulong mainChannelId;

                if (mainChannel.Value.Value == null) //this is a weird struct...
                {
                    mainChannelId = channelId;
                }
                else
                {
                    mainChannelId = mainChannel.Value.Value.ChannelId;
                }

                var channel = new ChannelPermission(mainChannelId, channelId, serverId, false);
                ChannelPermissions.Add(channelId, channel);
                SaveChannelID();
            }
        }

        private void SaveChannelID()
        {
            FileHelper.WriteToFile(ChannelPermissions.Keys.Select(s => s.ToString()).ToList(),
                Constants.REGISTERED_CHANNEL_FILE);
        }

        public void SetMainChannelForServer(ulong serverId, ulong channelId)
        {
            var channels = ChannelPermissions.Where(s => s.Value.ServerId == serverId).ToList();
            if (channels.Count == 0)// I am actually not even sure if this can be called
            {
                CheckAndCreateChannel(serverId, channelId);
                return;
            }
            foreach (var channel in channels)
            {
                channel.Value.MainChannelId = channelId;
                FileHelper.WriteObjectToFile(Path.Combine(Constants.CHANNELS_OPTIONS_FOLDER, channel.Key.ToString()), channel.Value);
            }
        }

        public ulong GetMainChannelForServer(ulong serverId)
        {
            var channel = ChannelPermissions?.FirstOrDefault(s => s.Value.ServerId == serverId);
            if (channel.Value.Value == null)
            {
                return 0;
            }
            return channel.Value.Value.MainChannelId;
        }

        public void SetUserPermission(ulong serverId, ulong channelId, ulong userId, PermissionLevel level)
        {
            CheckAndCreateChannel(serverId, channelId);
            var channel = ChannelPermissions[channelId];
            if (channel.UserPermissions.ContainsKey(userId))
            {
                channel.UserPermissions[userId] = level;
            }
            else
            {
                channel.UserPermissions.Add(userId, level);
            }

            FileHelper.WriteObjectToFile(Path.Combine(Constants.CHANNELS_OPTIONS_FOLDER, channelId.ToString()), channel);
        }

        public void SetChannelMuting(ulong serverId, ulong channelId, bool muted)
        {
            CheckAndCreateChannel(serverId, channelId);
            var channel = ChannelPermissions[channelId];
            channel.Muted = muted;
            FileHelper.WriteObjectToFile(Path.Combine(Constants.CHANNELS_OPTIONS_FOLDER, channelId.ToString()), channel);
        }

        public bool ContainsChannel(ulong channelId)
        {
            return ChannelPermissions.ContainsKey(channelId);
        }

        public bool IsUserInChannel(ulong channelId, ulong userId)
        {
            if (!ContainsChannel(channelId))
            {
                return false;
            }
            return ChannelPermissions[channelId].UserPermissions.ContainsKey(userId);
        }

        public PermissionLevel GetUserPermission(ulong channelId, ulong userId)
        {
            if (!IsUserInChannel(channelId, userId))
            {
                return PermissionLevel.User;
            }
            return ChannelPermissions[channelId].UserPermissions[userId];
        }

        public bool IsDeveloper(ulong channelId, ulong userId)
        {
            if (!ContainsChannel(channelId))
            {
                return false;
            }
            return GetUserPermission(channelId, userId) == PermissionLevel.Developer;
        }
    }
}
