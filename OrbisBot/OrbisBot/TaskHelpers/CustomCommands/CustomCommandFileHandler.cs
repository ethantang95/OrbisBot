using OrbisBot.Permission;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.TaskHelpers.CustomCommands
{
    static class CustomCommandFileHandler
    {
        //for custom commands, this will save the commands into a specific custom commands folder
        //this class will handle the values from the file and create the appropriate Custom Commands class
        //however, the custom commands class will handle its own output

        public static void SaveCustomTask(List<CustomCommandForm> toSave)
        {
            FileHelper.WriteObjectToFile(Path.Combine(Constants.CUSTOM_COMMANDS_FOLDER, toSave[0].CommandName + ".txt"), toSave);
        }

        public static List<CustomCommandForm> GetCustomTaskFromFile(string taskFile)
        {
            var fileContents = FileHelper.GetObjectFromFile<List<CustomCommandForm>>(Path.Combine(Constants.CUSTOM_COMMANDS_FOLDER, taskFile));

            return fileContents;
        }

        public static void RemoveTaskFile(string taskFile)
        {
            FileHelper.RemoveFile(Path.Combine(Constants.CUSTOM_COMMANDS_FOLDER, taskFile));
        }
    }
}
