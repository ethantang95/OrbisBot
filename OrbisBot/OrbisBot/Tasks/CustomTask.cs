using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using OrbisBot.TaskHelpers.CustomCommands;

namespace OrbisBot.Tasks
{
    class CustomTask : SingleChannelTaskAbstract
    {
        private string _commandText;
        private int _maxArgs;
        private string[] _returnValues;
        public CustomTask(string commandName, int maxArgs, string[] returnValues, long channel)
        {
            _commandText = commandName;
            _maxArgs = maxArgs;
            _returnValues = returnValues;
            _commandPermission.ChannelPermissionLevel.Add(channel, PermissionLevel.User);
        }
        public override string AboutText()
        {
            return "A customized task created for your channel by a user";
        }

        public override string CommandText()
        {
            return _commandText;
        }

        public override CommandPermission DefaultCommandPermission()
        {
            return new CommandPermission(false, PermissionLevel.User, false);
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            if (args.Length != _maxArgs + 1)
            {
                return $"Not enough parameters was supplied for this command, it requires {_maxArgs} parameters.";
            }

            var mentioned = messageSource.Message.MentionedUsers;

            //we will first parse the args into mentioned
            args = args.Select(s => s[0] == '@' && mentioned.FirstOrDefault(r => r.Name == s.Substring(1)) != null ? Mention.User(mentioned.First(r => r.Name == s.Substring(1))) : s).ToArray();
            
            var selectedLine = _returnValues[new Random().Next(0, _returnValues.Length)]; //dammit, why the hell isn't it inclusive

            var commandArgs = args.Skip(1).ToArray();

            var builder = new CustomCommandBuilder(selectedLine, commandArgs, messageSource.User.Name);

            return builder.GenerateString();
        }
    }
}
