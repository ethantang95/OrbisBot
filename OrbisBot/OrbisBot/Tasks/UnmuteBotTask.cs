using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using OrbisBot.TaskAbstracts;

namespace OrbisBot.Tasks
{
    class UnmuteBotTask : FilePermissionTaskAbstract
    {
        public override string AboutText()
        {
            return "Unmute the bot in this channel and optionally in this entire server. Note: both muting and unmuting the bot uses the same permission file.";
        }

        public override string CommandText()
        {
            return "unmutebot";
        }

        public override CommandPermission DefaultCommandPermission()
        {
            return new CommandPermission(false, PermissionLevel.Admin, true);
        }

        public override string PermissionFileSource()
        {
            return Constants.MUTE_BOT_FILE;
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            if (args.Length > 2)
            {
                return SyntaxStructure();
            }

            if (args.Length == 1)//we are muting the currrent channel only
            {
                Context.Instance.ChannelPermission.SetChannelMuting(messageSource.Server.Id, messageSource.Channel.Id, false);

                return $"{messageSource.Channel.Name} is now unmuted from OrbisBot.";
            }
            else if (args[1].Equals("server", StringComparison.InvariantCultureIgnoreCase))
            {
                var serverIds = messageSource.Server.Channels.Select(s => s.Id).ToList();
                serverIds.ForEach(s => Context.Instance.ChannelPermission.SetChannelMuting(messageSource.Server.Id, s, false));

                return $"{messageSource.Server.Name} is now unmuted from OrbisBot.";
            }

            return SyntaxStructure();
        }

        private string SyntaxStructure()
        {
            return $"{Constants.SYNTAX_INTRO} OPTIONAL Server. Type the word server after the command to unmute the entire server";
        }

        public override string ExceptionMessage(Exception ex, MessageEventArgs eventArgs)
        {
            return "An error has occurred when trying to unmute the bot, the developers has been notified of this problem";
        }
    }
}
