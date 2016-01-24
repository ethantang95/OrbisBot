﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using OrbisBot.TaskHelpers.CustomCommands;
using OrbisBot.TaskAbstracts;

namespace OrbisBot.Tasks
{
    class CustomTask : RegisteredChannelTaskAbstract
    {
        private string _commandText;
        private Dictionary<long, CustomCommandForm> _customCommands;
        public CustomTask(string commandName, List<CustomCommandForm> commands)
        {
            _commandText = commandName;
            _customCommands = commands.ToDictionary(s => s.Channel, s => s);
            commands.ForEach(s => _commandPermission.ChannelPermission.Add(s.Channel, new ChannelPermissionSetting(DefaultCommandPermission().DefaultLevel, DefaultCommandPermission().DefaultCoolDown)));
        }
        public override string AboutText()
        {
            return "A customized task created for your channel by a user";
        }

        public override string CommandText()
        {
            return _commandText.ToLower();
        }

        public override CommandPermission DefaultCommandPermission()
        {
            return new CommandPermission(false, PermissionLevel.User, false, 30);
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            var command = _customCommands[messageSource.Channel.Id];

            if (args.Length < command.MaxArgs + 1)
            {
                return $"Not enough parameters was supplied for this command, it requires {command.MaxArgs} parameters.";
            }
            else if (args.Length > command.MaxArgs + 1)
            {
                return $"Too many parameters are supplied to this command. If you are trying to input a name or a statement, surround it with \". This command requires {command.MaxArgs} parameters ";
            }

            var mentioned = messageSource.Message.MentionedUsers;

            //we will first parse the args into mentioned
            args = args.Select(s => s[0] == '@' && mentioned.FirstOrDefault(r => r.Name == s.Substring(1)) != null ? Mention.User(mentioned.First(r => r.Name == s.Substring(1))) : s).ToArray();
            
            var selectedLine = command.ReturnValues[new Random().Next(0, command.ReturnValues.Count)]; //dammit, why the hell isn't it inclusive

            var commandArgs = args.Skip(1).ToArray();

            var builder = new CustomCommandBuilder(selectedLine, commandArgs, messageSource.User.Name, messageSource.Channel.Members);

            return builder.GenerateCustomMessage();
        }

        public void AddContent(CustomCommandForm toAdd)
        {
            if (!_customCommands.ContainsKey(toAdd.Channel))
            {
                _customCommands.Add(toAdd.Channel, toAdd);
                _commandPermission.ChannelPermission.Add(toAdd.Channel, new ChannelPermissionSetting(toAdd.PermissionLevel, toAdd.CoolDown));
            }
            else
            {
                _customCommands[toAdd.Channel] = toAdd;
            }
            CustomCommandFileHandler.SaveCustomTask(GetCustomCommands());
        }

        public void RemoveCommand(long channelId)
        {
            _customCommands.Remove(channelId);
            _commandPermission.ChannelPermission.Remove(channelId);
            if (_customCommands.Count == 0)
            {
                CustomCommandFileHandler.RemoveTaskFile(_commandText + ".txt");
                Context.Instance.Tasks.Remove(this.CommandTrigger());
            }
            else
            {
                CustomCommandFileHandler.SaveCustomTask(GetCustomCommands());
            }
        }

        public List<CustomCommandForm> GetCustomCommands()
        {
            var toReturn = _customCommands.Values.ToList();

            return toReturn;
        }

        public override bool CheckArgs(string[] args)
        {
            //for this command, it is too dynamic to determine if the amount of args are enough
            //have it inside the actual task to check for arguments
            return true;
        }

        public override string UsageText()
        {
            return "This is a custom command created for your channel, the usage might vary";
        }

        public override void SaveSettings(CommandPermission commandPermission)
        {
            foreach (var channelPermission in commandPermission.ChannelPermission)
            {
                _customCommands[channelPermission.Key].PermissionLevel = channelPermission.Value.PermissionLevel;
                _customCommands[channelPermission.Key].CoolDown = channelPermission.Value.CoolDown;
            }
            CustomCommandFileHandler.SaveCustomTask(GetCustomCommands());
        }
    }
}
