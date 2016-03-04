using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.TaskHelpers.AdminUtils
{
    class GeneralAdminUtils
    {
        public static bool IsCommandChannel(ulong id)
        {
            return UInt64.Parse(ConfigurationManager.AppSettings[Constants.COMMAND_CHANNEL]) == id;
        }
    }
}
