using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot
{
    static class CommonTools
    {
        public static long ToUnixMilliTime(DateTime time)
        {
            return (time.ToUniversalTime().Ticks - 621355968000000000) / TimeSpan.TicksPerMillisecond;
        }

        public static long ToWindowsTicks(long ticks)
        {
            return (ticks * TimeSpan.TicksPerMillisecond) + 621355968000000000;
        }
    }
}
