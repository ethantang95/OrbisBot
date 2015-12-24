using OrbisBot.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.Tasks { 

    //this class exists for tasks that belongs to a single channel
    abstract class SingleChannelTaskAbstract : TaskAbstract
    {
        public override PermissionLevel GetCommandPermissionForChannel(long channelId)
        {
            throw new NotImplementedException();
        }

        public override void SetCommandPermissionForChannel(long channelId, PermissionLevel newPermissionLevel)
        {
            throw new NotImplementedException();
        }

        public new void SetPermission(long channelId, PermissionLevel level)
        {
        }
    }
}
