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
        public static string Execute(string[] args, MessageEventArgs eventArgs, int iterations = 0)
        {
            //the first one i the name of the command to execute
            if (!Context.Instance.Tasks.ContainsKey(args[0].ToLower()))
            {
                throw new ArgumentException($"The command {args[0]} does not exist");
            }
            var task = Context.Instance.Tasks[args[0].ToLower()];
            var result = task.ExecuteTaskDirect(args, eventArgs, iterations);
            return result;
        }
    }
}
