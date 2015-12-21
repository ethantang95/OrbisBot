using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot
{
    class CommandParser
    {

        public static string[] ParseCommand(string command)
        {
            //the syntax we will have is always !command. Each segment separated by a space will represent a part of the args
            //any content inside "<query>" is a query string and is part of the args
            //any content inside [<list>] is part of a list, what is inside the list are comma separated
            //we will just iterate through the string

            var parsedArgs = new List<string>();
            var lastPosition = 0;
            var insideQuery = false;
            var insideList = false;
            for (int i = 0; i < command.Length; i++)
            {
                if ((command[i] == ' ' || i == command.Length - 1) && !insideQuery && !insideList)
                {
                    //we detected a space
                    //edge case, if we are at the end of a command
                    if (i == command.Length - 1)
                    {
                        i++;
                    }
                    var argWord = command.Substring(lastPosition, i - lastPosition);
                    if (!String.IsNullOrWhiteSpace(argWord))
                    {
                        argWord = argWord.Trim();
                        parsedArgs.Add(argWord);
                    }
                    lastPosition = i + 1;
                }
                else if (command[i] == '"' && !insideList)
                {
                    if (!insideQuery)
                    {
                        insideQuery = true;
                        lastPosition = i;
                    }
                    else
                    {
                        //we are at the end of the query
                        var argQuery = command.Substring(lastPosition, i - lastPosition + 1);
                        argQuery = argQuery.Replace("\"", "").Trim();
                        parsedArgs.Add(argQuery);
                        lastPosition = i + 1;
                        insideQuery = false;
                    }
                }
                else if (command[i] == '[' && !insideQuery && !insideList)
                {
                    lastPosition = i;
                    insideList = true;
                }
                else if (command[i] == ']' && insideList)
                {
                    var argList = command.Substring(lastPosition, i - lastPosition + 1);
                    argList = argList.Substring(1, argList.Length - 2).Trim(); //remove the backets
                    parsedArgs.Add(argList);
                    lastPosition = i + 1;
                    insideList = false;
                }
            }

            return parsedArgs.ToArray();
        }
    }
}
