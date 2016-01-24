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
        public long MainChannelId { get; set; }
        public long ChannelId { get; private set; }
        public long ServerId { get; private set; }
        public bool Muted { get; set; }
        public Dictionary<long, PermissionLevel> UserPermissions { get; set; }

        public ChannelPermission(long mainChannelId, long channelId, long serverId, bool muted)
        {
            MainChannelId = mainChannelId;
            ChannelId = channelId;
            ServerId = serverId;
            Muted = muted;
            UserPermissions = new Dictionary<long, PermissionLevel>();
        }
    }
}
