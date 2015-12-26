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
        private Dictionary<long, CustomCommandForm> _customCommands; 
        public CustomTask(string commandName, List<CustomCommandForm> commands)
        {
            _commandText = commandName;
            _customCommands = commands.ToDictionary(s => s.Channel, s => s);
            commands.ForEach(s => _commandPermission.ChannelPermissionLevel.Add(s.Channel, s.PermissionLevel));
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
            //first, get the appropriate form... keys should be contained because if not, it will be
            //denied access to this component

            var command = _customCommands[messageSource.Channel.Id];

            if (args.Length != command.MaxArgs + 1)
            {
                return $"Not enough parameters was supplied for this command, it requires {command.MaxArgs} parameters.";
            }

            var mentioned = messageSource.Message.MentionedUsers;

            //we will first parse the args into mentioned
            args = args.Select(s => s[0] == '@' && mentioned.FirstOrDefault(r => r.Name == s.Substring(1)) != null ? Mention.User(mentioned.First(r => r.Name == s.Substring(1))) : s).ToArray();
            
            var selectedLine = command.ReturnValues[new Random().Next(0, command.ReturnValues.Count)]; //dammit, why the hell isn't it inclusive

            var commandArgs = args.Skip(1).ToArray();

            var builder = new CustomCommandBuilder(selectedLine, commandArgs, messageSource.User.Name);

            return builder.GenerateString();
        }

        public void AddContent(CustomCommandForm toAdd)
        {
            if (!_customCommands.ContainsKey(toAdd.Channel))
            {
                _customCommands.Add(toAdd.Channel, toAdd);
                _commandPermission.ChannelPermissionLevel.Add(toAdd.Channel, toAdd.PermissionLevel);
            }
            else
            {
                _customCommands[toAdd.Channel] = toAdd;
            }
            CustomCommandFileHandler.SaveCustomTask(ToFileOutput());
        }

        public void RemoveCommand(long channelId)
        {
            _customCommands.Remove(channelId);
            _commandPermission.ChannelPermissionLevel.Remove(channelId);
        }

        public List<string> ToFileOutput()
        {
            var toReturn = new List<string>();
            //the command will be the name of the file
            toReturn.Add($"{Constants.COMMAND_NAME}:{_commandText}");
            _customCommands.Select(s => s.Value).ToList().ForEach(s =>
            {
                toReturn.Add($"{Constants.MAX_ARGS}:{s.MaxArgs.ToString()}");
                toReturn.Add($"{Constants.CHANNEL_ID}:{s.Channel.ToString()}");
                toReturn.Add($"{Constants.PERMISSION_LEVEL}:{s.PermissionLevel.ToString()}");
                s.ReturnValues.ForEach(r => toReturn.Add($"{Constants.RETURN_TEXT}:{r}"));
            });

            return toReturn;
        }
    }
}
