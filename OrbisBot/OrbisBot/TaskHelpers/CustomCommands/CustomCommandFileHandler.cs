using OrbisBot.Permission;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.TaskHelpers.CustomCommands
{
    class CustomCommandFileHandler
    {
        //for custom commands, this will save the commands into a specific custom commands folder
        //this class will handle the values from the file and create the appropriate Custom Commands class
        //however, the custom commands class will handle its own output

        public static void SaveCustomTask(List<string> toSave)
        {
            var commandName = toSave[0].Split(':');
            FileHelper.WriteToFile(toSave, Path.Combine(Constants.CUSTOM_COMMANDS_FOLDER, commandName[1] + ".txt"));
        }

        public static List<CustomCommandForm> GetCustomTaskFromFile(string taskFile)
        {
            var fileContents = FileHelper.GetContentFromFile(Path.Combine(Constants.CUSTOM_COMMANDS_FOLDER, taskFile));

            List<CustomCommandForm> toReturn = new List<CustomCommandForm>();

            var taskName = fileContents[0].Split(':')[1];

            int maxArgs = 0;
            long channelID = 0;
            PermissionLevel level = PermissionLevel.Restricted;
            List<string> returnValue = new List<string>();
            //we will just iterate through this
            for (int i = 0; i < fileContents.Count; i++)
            {
                var line = fileContents[i].Split(new char[] { ':' }, 2);
                if (line[0] == Constants.MAX_ARGS) //well, every entry is at least 4 lines
                {
                    if (i > 2)
                    {
                        toReturn.Add(new CustomCommandForm(taskName, maxArgs, channelID, level, returnValue));
                        returnValue = new List<string>();
                    }
                    maxArgs = Int32.Parse(line[1]);
                }
                else if (line[0] == Constants.CHANNEL_ID)
                {
                    channelID = Int64.Parse(line[1]);
                }
                else if (line[0] == Constants.PERMISSION_LEVEL)
                {
                    level = PermissionEnumMethods.ParseString(line[1]);
                }
                else if (line[0] == Constants.RETURN_TEXT)
                {
                    returnValue.Add(line[1]);
                }
            }
            //if edge case for the last one
            toReturn.Add(new CustomCommandForm(taskName, maxArgs, channelID, level, returnValue));

            return toReturn;
        }
    }
}
