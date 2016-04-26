using Discord;
using OrbisBot.OrbScript;
using OrbisBot.ServerSettings;
using OrbisBot.TaskHelpers.CustomCommands;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot
{
    class Program
    {
        static void Main(string[] args)
        {
            //check if the required directories exists
            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Constants.ORBIS_BOT_NAME)))
            {
                Directory.CreateDirectory(Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Constants.ORBIS_BOT_NAME));
                Console.WriteLine("Created folder for orbis bot");
            }

            CheckAndCreateFolders(Constants.CUSTOM_COMMANDS_FOLDER);
            CheckAndCreateFolders(Constants.CHANNELS_OPTIONS_FOLDER);
            CheckAndCreateFolders(Constants.SERVER_OPTIONS_FOLDER);
            CheckAndCreateFolders(Constants.GLOBAL_SETTINGS_FOLDER);
            CheckAndCreateFolders(Constants.OPTIONS_FOLDER);
            CheckAndCreateFolders(Constants.PROFILES_FOLDER);

            var app = Context.Instance;

            //PortOldToNewEngine();
        }

        public static void CheckAndCreateFolders(string folder)
        {
            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Constants.ORBIS_BOT_NAME, folder)))
            {
                Directory.CreateDirectory(Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Constants.ORBIS_BOT_NAME, folder));
                Console.WriteLine($"Created OrbisBot Folder {folder}");
            }
        }

        public static void PortOldToNewEngine()
        {
            var allFiles = FileHelper.GetAllFiles(Constants.CUSTOM_COMMANDS_FOLDER);
            foreach (var filePath in allFiles)
            {
                var customTaskContents = CustomCommandFileHandler.GetCustomTaskFromFile(filePath);

                var newList = new List<CustomCommandForm>();

                foreach (var content in customTaskContents)
                {
                    var newContent = new CustomCommandForm(content.CommandName, content.MaxArgs, content.Channel, content.PermissionLevel, new List<string>(), content.CoolDown);
                    foreach (var value in content.ReturnValues)
                    {
                        var newValue = value;
                        //first replace the params
                        for (int i = 0; i < newContent.MaxArgs; i++) {
                            newValue = newValue.Replace($"%{i + 1}u", $"~FindAndMentionUser($param{i + 1})");
                            newValue = newValue.Replace($"%{i + 1}", $"$param{i + 1}");
                        }

                        //replace the user mentions
                        newValue = newValue.Replace("%uu", "~MentionUser()");
                        newValue = newValue.Replace("%u", "$user");

                        //replace function calls
                        newValue = newValue.Replace("%c(", "~Execute(");

                        //replace the randoms
                        newValue = newValue.Replace("%r(", "~Random(");
                        newValue = newValue.Replace("http,", "http:");

                        newContent.ReturnValues.Add(newValue);
                    }

                    newList.Add(newContent);
                }

                //write to file
                CustomCommandFileHandler.SaveCustomTask(newList);
            }

            //now deal with the server welcome messages
            var serverSetting = new ServerSettingsWrapper();

            foreach (var server in serverSetting.ServerSettings)
            {
                var message = server.Value.WelcomeMsg;

                var newValue = message;

                //replace the user mentions
                newValue = newValue.Replace("%uu", "~MentionUser()");
                newValue = newValue.Replace("%u", "$user");

                //replace function calls
                newValue = newValue.Replace("%c(", "~Execute(");

                //replace the randoms
                newValue = newValue.Replace("%r(", "~Random(");
                newValue = newValue.Replace(":", ",");

                serverSetting.SetWelcomeMessage(server.Key, newValue);
            }
        }
    }
}
