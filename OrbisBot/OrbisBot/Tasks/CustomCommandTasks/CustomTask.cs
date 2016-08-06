using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using OrbisBot.Permission;
using OrbisBot.TaskHelpers.CustomCommands;
using OrbisBot.TaskAbstracts;
using OrbisBot.OrbScript;
using OrbisBot.TaskPermissions;

namespace OrbisBot.Tasks
{
    class CustomTask : TaskAbstract
    {
        private string _commandText;
        private Dictionary<ulong, CustomCommandForm> _customCommands;
        public CustomTask(string commandName, List<CustomCommandForm> commands, RegisteredChannelTaskPermission<CustomCommandForm> permission) : base(permission)
        {
            _commandText = commandName;
            _customCommands = commands.ToDictionary(s => s.Channel, s => s);
        }
        public override string AboutText()
        {
            return "A customized task created for your channel by a user";
        }

        public override string CommandText()
        {
            return _commandText.ToLower();
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
            args = args.Select(s => s[0] == '@' && mentioned.FirstOrDefault(r => r.Name == s.Substring(1)) != null ? mentioned.First(r => r.Name == s.Substring(1)).Mention : s).ToArray();
            
            var selectedLine = command.ReturnValues[new Random().Next(0, command.ReturnValues.Count)]; //dammit, why the hell isn't it inclusive

            var commandArgs = args.Skip(1).ToArray();

            var iterations = HasVariable(messageSource.Channel.Id, "iterations") ? (int)GetVariable(messageSource.Channel.Id, "iterations") : 0;

            var engineConfig = new OrbScriptConfiger(messageSource.User, OrbScriptBuildType.Standard)
                .SetEventArgs(messageSource)
                .SetIgnoreList(Context.Instance.GlobalSetting.HideList)
                .SetRoleList(messageSource.Server.Roles)
                .SetUserList(messageSource.Channel.Users)
                .SetCallIterations(iterations)
                .SetSourceCommand(CommandText());

            var engine = new OrbScriptEngine(engineConfig);
            engine.SetArgs(commandArgs);

            string result;

            try
            {
                result = engine.EvaluateString(selectedLine);
            }
            catch (Exception e)
            {
                result = $"An error has occurred when trying to parse the script {selectedLine} with the error {e.Message}";
            }

            return result;
            
        }

        public void AddContent(CustomCommandForm toAdd)
        {
            if (!_customCommands.ContainsKey(toAdd.Channel))
            {
                _customCommands.Add(toAdd.Channel, toAdd);
                TaskPermission.AddPermission(toAdd);
            }
            else
            {
                _customCommands[toAdd.Channel] = toAdd;
                TaskPermission.UpdatePermission(toAdd);
            }
        }

        public void RemoveCommand(ulong channelId)
        {
            _customCommands.Remove(channelId);
            TaskPermission.RemovePermission(channelId);
            if (_customCommands.Count == 0)
            {
                CustomCommandFileHandler.RemoveTaskFile(_commandText + ".txt");
                Context.Instance.Tasks.Remove(CommandText());
            }
        }

        public void UpdateCommand(string[] newCommands, MessageEventArgs source)
        {
            var content = _customCommands[source.Channel.Id];

            content.ReturnValues.AddRange(newCommands);

            TaskPermission.UpdatePermission(content);
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
    }
}
