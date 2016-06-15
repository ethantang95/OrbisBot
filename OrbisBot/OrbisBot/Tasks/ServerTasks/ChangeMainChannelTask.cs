using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using OrbisBot.TaskAbstracts;
using OrbisBot.TaskPermissions;

namespace OrbisBot.Tasks
{
    class ChangeMainChannelTask : TaskAbstract
    {
        public ChangeMainChannelTask(FileBasedTaskPermission permission) : base(permission)
        {

        }

        public override string AboutText()
        {
            return "Set the current channel as the main channel for this server, all general messages by OrbisBot will be sent to this channel";
        }

        public override string CommandText()
        {
            return "channel-setasmain";
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            //this should be moved to the server settings now
            Context.Instance.ChannelPermission.SetMainChannelForServer(messageSource.Server.Id, messageSource.Channel.Id);

            return $"Successfully changed the main channel to {messageSource.Channel.Name}";
        }

        public override string ExceptionMessage(Exception ex, MessageEventArgs eventArgs)
        {
            return "An error has occurred when trying to change the main channel, the developers has been notified of this problem";
        }

        public override bool CheckArgs(string[] args)
        {
            return true;
        }

        public override string UsageText()
        {
            return Constants.NO_PARAMS_USAGE;
        }
    }
}
