using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.OrbScript.Functions
{
    static class ScriptCastFunctions
    {
        public static string BoolToNum(string boolVal)
        {
            var b = bool.Parse(boolVal);
            return (b ? 1 : 0).ToString();
        }

        public static string NumToBool(string numVal)
        {
            var num = double.Parse(numVal);
            return (num != 0 ? true : false).ToString();
        }

        public static string TimeToUnix(string timeStr)
        {
            var date = DateTime.Parse(timeStr);

            var ticks = CommonTools.ToUnixMilliTime(date);

            return ticks.ToString();
        }

        public static string UnixToTime(string unixTicks)
        {
            var ticks = long.Parse(unixTicks);

            var winTicks = CommonTools.ToWindowsTicks(ticks);

            var date = new DateTime(winTicks);

            return date.ToString();
        }
    }
}
