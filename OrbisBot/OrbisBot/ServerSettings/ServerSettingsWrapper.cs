using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.ServerSettings
{
    class ServerSettingsWrapper
    {
        public Dictionary<long, ServerSetting> ServerSettings { get; private set; }

        public ServerSettingsWrapper()
        {
            ServerSettings = new Dictionary<long, ServerSetting>();
            GetAllRegisteredServers();
        }

        private void GetAllRegisteredServers()
        {
            var servers = FileHelper.GetContentFromFile(Constants.REGISTERED_SERVER_FILE);
            if (servers != null)
            {
                servers.ForEach(s => SetServerSettings(Int64.Parse(s)));
            }
        }

        private void SetServerSettings(long serverId)
        {
            var serverFile = FileHelper.GetValuesFromFile(Path.Combine(Constants.SERVER_OPTIONS_FOLDER, serverId.ToString()));

            if (serverFile == null)
            {
                return;
            }

            var serverSettings = new ServerSetting(Int64.Parse(serverFile[Constants.SERVER_ID]), bool.Parse(serverFile[Constants.ENABLE_WELCOME]), serverFile[Constants.WELCOME_MSG]);

            ServerSettings.Add(serverId, serverSettings);
        }

        private void CheckAndCreateServer(long serverId)
        {
            if (!ServerSettings.ContainsKey(serverId))
            {
                var serverSetting = new ServerSetting(serverId, false, string.Empty);

                ServerSettings.Add(serverId, serverSetting);

                SaveServerID();
            }
        }

        private void SaveServerID()
        {
            FileHelper.WriteToFile(ServerSettings.Keys.Select(s => s.ToString()).ToList(), Constants.REGISTERED_SERVER_FILE);
        }

        public void SetWelcomeEnable(long serverId, bool welcomeEnable)
        {
            CheckAndCreateServer(serverId);
            var server = ServerSettings[serverId];
            server.EnableWelcome = welcomeEnable;
            FileHelper.WriteValuesToFile(server.toFileOutput(), Path.Combine(Constants.SERVER_OPTIONS_FOLDER, serverId.ToString()));
        }

        public void SetWelcomeMessage(long serverId, string welcomeMessage)
        {
            CheckAndCreateServer(serverId);
            var server = ServerSettings[serverId];
            server.WelcomeMsg = welcomeMessage;
            FileHelper.WriteValuesToFile(server.toFileOutput(), Path.Combine(Constants.SERVER_OPTIONS_FOLDER, serverId.ToString()));
        }

        public string GetWelcomeMessage(long serverId)
        {
            CheckAndCreateServer(serverId);
            return ServerSettings[serverId].WelcomeMsg;
        }
    }
}
