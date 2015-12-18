using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.Permission
{
    //this exists for the purpose of each channel, this will contain each individual's permission settings
    class ChannelPermission
    {
        public long ChannelId { get; private set; }
        public long ServerId { get; private set; }
        public bool Muted { get; set; }
        public Dictionary<long, PermissionLevel> UserPermissions { get; set; }

        public ChannelPermission(long channelId, long serverId, bool muted)
        {
            ChannelId = channelId;
            ServerId = serverId;
            Muted = muted;
            UserPermissions = new Dictionary<long, PermissionLevel>();
        }

        public Dictionary<string, string> toFileOutput()
        {
            Dictionary<string, string> toReturn = new Dictionary<string, string>();
            toReturn.Add(Constants.CHANNEL_ID, ChannelId.ToString());
            toReturn.Add(Constants.SERVER_ID, ServerId.ToString());
            toReturn.Add(Constants.CHANNEL_MUTED, Muted.ToString());
            UserPermissions.ToList().ForEach(s => toReturn.Add(s.Key.ToString(), s.Value.ToString()));

            return toReturn;
        }
    }
}
