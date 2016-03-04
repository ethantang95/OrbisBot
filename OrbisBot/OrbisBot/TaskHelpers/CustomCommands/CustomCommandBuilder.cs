using Discord;
using OrbisBot.TaskHelpers.UserFinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.TaskHelpers.CustomCommands
{
    class CustomCommandBuilder
    {
        private string _baseCommand;
        private string[] _args;
        private string _userName;
        private IEnumerable<User> _users;

        public CustomCommandBuilder(string baseCommand, string[] args, string userName, IEnumerable<User> users)
        {
            _baseCommand = baseCommand;
            _args = args;
            _userName = userName;
            _users = users;
        }

        public void ReplaceIndependents()
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

        public void EvaluateTokens()
        {
            int iterations = 0; //we are setting it to a max of 1000 iterations, if it hits that
            //it means there's a circular dependency problem... which sucks
            while (CustomCommandUtils.ContainsTokens(_baseCommand) && iterations < 1000)
            {
                for (int i = 0; i < _baseCommand.Length - 1; i++) //bit hacky, but really to avoid IndexOutOfBounds as tokens are at least 2 chars in length
                {
                    //find a token
                    if (_baseCommand[i] == '%')
                    {
                        if (_baseCommand[i + 1] == 'r') //token represents random
                        {
                            var innerContent = CustomCommandUtils.ExtractInnerContent(_baseCommand, i);
                            var generatedValue = CustomCommandUtils.GetRandomValueForToken(innerContent);
                            _baseCommand = CustomCommandUtils.ReplaceTokenWithValue(_baseCommand, i, generatedValue);
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

        public string GenerateCustomMessage()
        {
            ReplaceIndependents();
            EvaluateTokens();
            return _baseCommand;
        }
    }
}
