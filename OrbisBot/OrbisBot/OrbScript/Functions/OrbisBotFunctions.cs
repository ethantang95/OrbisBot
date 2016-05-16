using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.OrbScript.Functions
{
    static class OrbisBotFunctions
    {
        public static string Execute(string argString, MessageEventArgs eventArgs, int iterations = 0)
        {
            var args = CommandParser.ParseCommand(argString);
            //the first one i the name of the command to execute
            if (!Context.Instance.Tasks.ContainsKey(args[0].ToLower()))
            {
                throw new ArgumentException($"The command {args[0]} does not exist");
            }
            var task = Context.Instance.Tasks[args[0].ToLower()];
            var result = task.ExecuteTaskDirect(args, eventArgs, iterations);
            return result;
        }

        public static string SetVariable(string name, string value, ulong channelId, string commandName)
        {
            var command = Context.Instance.Tasks[commandName]; //it should exist, it must exist

            command.SetVariable(channelId, name, value);

            return string.Empty;
        }

        public static string SetUserVariable(string name, string value, ulong channelId, ulong userId, string commandName)
        {
            var command = Context.Instance.Tasks[commandName]; //it should exist, it must exist

            command.SetUserVariable(channelId, userId, name, value);

            return string.Empty;
        }

        public static string GetVariable(string name, string defaultVal, ulong channelId, string commandName)
        {
            var command = Context.Instance.Tasks[commandName];

            if (command.HasVariable(channelId, name))
            {
                return (string)command.GetVariable(channelId, name);
            }
            else
            {
                return defaultVal;
            }
        }

        public static string GetUserVariable(string name, string defaultVal, ulong channelId, ulong userId, string commandName)
        {
            var command = Context.Instance.Tasks[commandName];

            if (command.HasUserVariable(channelId, userId, name))
            {
                return (string)command.GetUserVariable(channelId, userId, name);
            }
            else
            {
                return defaultVal;
            }
        }
    }
}
