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

            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Constants.ORBIS_BOT_NAME, Constants.CHANNELS_OPTIONS_FOLDER)))
            {
                Directory.CreateDirectory(Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Constants.ORBIS_BOT_NAME, Constants.CHANNELS_OPTIONS_FOLDER));
                Console.WriteLine("Created folder for orbis bot's channel information");
            }

            var app = Context.Instance;
        }
    }
}
