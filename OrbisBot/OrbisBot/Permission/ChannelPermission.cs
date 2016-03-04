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
        public ulong MainChannelId { get; set; }
        public ulong ChannelId { get; private set; }
        public ulong ServerId { get; private set; }
        public bool Muted { get; set; }
        public Dictionary<ulong, PermissionLevel> UserPermissions { get; set; }

        public ChannelPermission(ulong mainChannelId, ulong channelId, ulong serverId, bool muted)
        {
            MainChannelId = mainChannelId;
            ChannelId = channelId;
            ServerId = serverId;
            Muted = muted;
            UserPermissions = new Dictionary<ulong, PermissionLevel>();
        }
    }
}
