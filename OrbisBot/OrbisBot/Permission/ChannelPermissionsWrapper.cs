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
        public Dictionary<long, ChannelPermission> ChannelPermissions { get; private set; }

        //the format I will have the file is one master file that contains the id of all the channels
        //then the channels file will be name by the ID of that
        public ChannelPermissionsWrapper()
        {
            ChannelPermissions = new Dictionary<long, ChannelPermission>();
            GetAllRegisteredChannel();
        }

        private void GetAllRegisteredChannel()
        {
            var channels = FileHelper.GetContentFromFile(Constants.REGISTERED_CHANNEL_FILE);
            if (channels != null)
            {
                channels.ForEach(s => SetChannelPermission(Int64.Parse(s)));
            }
        }

        private void SetChannelPermission(long channelId)
        {
            var channelSettings = FileHelper.GetValuesFromFile(Path.Combine(Constants.CHANNELS_OPTIONS_FOLDER, channelId.ToString()));

            //The file will contain the channel ID, then the server ID, then is it muted or not
            //Rest will be individual's permission levels

            var channelPermission = new ChannelPermission(Int64.Parse(channelSettings[Constants.CHANNEL_ID]),
                    Int64.Parse(channelSettings[Constants.SERVER_ID]),
                    bool.Parse(channelSettings[Constants.CHANNEL_MUTED]));

            channelSettings.Remove(Constants.CHANNEL_ID);
            channelSettings.Remove(Constants.SERVER_ID);
            channelSettings.Remove(Constants.CHANNEL_MUTED);

            var permissionList = channelSettings.ToDictionary(s => Int64.Parse(s.Key), s => PermissionEnumMethods.ParseString(s.Value));

            channelPermission.UserPermissions = permissionList;

            ChannelPermissions.Add(channelId, channelPermission);
        }

        private void CheckAndCreateChannel(long serverId, long channelId)
        {
            if (!ChannelPermissions.ContainsKey(channelId))
            {
                var channel = new ChannelPermission(channelId, serverId, false);
                ChannelPermissions.Add(channelId, channel);
            }
        }

        private void SaveChannelID()
        {
            FileHelper.WriteToFile(ChannelPermissions.Keys.Select(s => s.ToString()).ToList(),
                Constants.REGISTERED_CHANNEL_FILE);
        }

        public void SetUserPermission(long serverId, long channelId, long userId, PermissionLevel level)
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
                SaveChannelID();
            }

            FileHelper.WriteValuesToFile(channel.toFileOutput(),
                Path.Combine(Constants.CHANNELS_OPTIONS_FOLDER, channelId.ToString()));
        }

        public void SetChannelMuting(long serverId, long channelId, bool muted)
        {
            CheckAndCreateChannel(serverId, channelId);
            var channel = ChannelPermissions[channelId];
            channel.Muted = muted;
            FileHelper.WriteValuesToFile(channel.toFileOutput(),
                Path.Combine(Constants.CHANNELS_OPTIONS_FOLDER, channelId.ToString()));
        }
    }
}
