using Discord;
using OrbisBot.TaskHelpers.UserFinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.TaskHelpers.CustomMessages
{
    class CustomMessageBuilder
    {
        private string _baseCommand;
        private string[] _args;
        private string _userName;
        private IEnumerable<User> _users;
        private IEnumerable<Role> _roles;

        public CustomMessageBuilder(string baseCommand, string[] args, string userName, IEnumerable<User> users, IEnumerable<Role> roles)
        {
            _baseCommand = baseCommand;
            _args = args;
            _userName = userName;
            _users = users;
            _roles = roles;
        }

        private void UsersTokenInjection()
        {
            //%l will be where the list will be injected
            var tokenReplacement = new StringBuilder();

            var userCount = _users.ToList().Count;
            for (int i = 1; i <= userCount; i++)
            {
                tokenReplacement.Append($"%{i}u");
            }

            _baseCommand = _baseCommand.Replace("%l", tokenReplacement.ToString());

            var everyoneRole = _roles.First(s => s.IsEveryone);

            _baseCommand = _baseCommand.Replace("%ev", everyoneRole.Mention);

            _args = _users.Select(s => s.Id.ToString()).ToArray();
        }

        private void ReplaceIndependents()
        {
            //we know that %1...N and %u are constants to be replaced
            _baseCommand = _baseCommand.Replace("%u", _userName);
            for (int i = 0; i < _args.Length; i++)
            {
                //first, we replace the user strings, if they are not found, replace it with norm strings
                var replaceUserString = "%" + (i + 1) + "u";
                var userToReplace = UserFinderUtil.FindUserMention(_users, _args[i]);
                _baseCommand = _baseCommand.Replace(replaceUserString, userToReplace);
                var replaceString = "%" + (i + 1);
                _baseCommand = _baseCommand.Replace(replaceString, _args[i]);
            }
        }

        private void EvaluateTokens()
        {
            int iterations = 0; //we are setting it to a max of 1000 iterations, if it hits that
            //it means there's a circular dependency problem... which sucks
            while (CustomMessageUtils.ContainsTokens(_baseCommand) && iterations < 1000)
            {
                for (int i = 0; i < _baseCommand.Length - 1; i++) //bit hacky, but really to avoid IndexOutOfBounds as tokens are at least 2 chars in length
                {
                    //find a token
                    if (_baseCommand[i] == '%')
                    {
                        if (_baseCommand[i + 1] == 'r') //token represents random
                        {
                            var innerContent = CustomMessageUtils.ExtractInnerContent(_baseCommand, i);
                            var generatedValue = CustomMessageUtils.GetRandomValueForToken(innerContent);
                            _baseCommand = CustomMessageUtils.ReplaceTokenWithValue(_baseCommand, i, generatedValue);
                            break;
                        }
                    }
                }
                iterations++;
            }

            if (iterations >= 1000)
            {
                throw new FormatException("The command you tried to use cannot construct a valid result. Please check the syntax and ensure there's no circular dependencies between the tokens.");
            }
        }

        //can only be called outside
        public CustomMessageBuilder EvaluateCommandTokens(MessageEventArgs messageEventArgs, int iterations = 0)
        {
            //command tokens are denoted by c, inside is the name of the command to be called including the dash
            for (int i = 0; i < _baseCommand.Length; i++)
            {
                if (_baseCommand[i] == '%' && _baseCommand[i + 1] == 'c')
                {
                    var commandRaw = CustomMessageUtils.ExtractInnerContent(_baseCommand, i + 1);
                    var commandList = CommandParser.ParseCommand(commandRaw);
                    if (!Context.Instance.Tasks.ContainsKey(commandList[0].ToLower()))
                    {
                        throw new ArgumentException($"The command {commandList[0]} does not exist");
                    }
                    var task = Context.Instance.Tasks[commandList[0].ToLower()];
                    var result = task.ExecuteTaskDirect(commandList, messageEventArgs, iterations);
                    _baseCommand = CustomMessageUtils.ReplaceTokenWithValue(_baseCommand, i, result);
                }
            }
            return this;
        }

        public CustomMessageBuilder GenerateGeneralMessage()
        {
            ReplaceIndependents();
            EvaluateTokens();
            return this;
        }

        public CustomMessageBuilder GenerateCalloutMessage()
        {
            UsersTokenInjection();
            GenerateGeneralMessage();
            return this;
        }

        public string GetMessage()
        {
            return _baseCommand;
        }
    }
}
