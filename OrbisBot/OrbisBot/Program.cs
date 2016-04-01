using Discord;
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
    }
}
