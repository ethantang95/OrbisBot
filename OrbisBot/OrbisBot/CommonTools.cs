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

        public static long ToUnixMilliTime(TimeSpan time)
        {
            return (time.Ticks / TimeSpan.TicksPerMillisecond);
        }

        public static long ToWindowsTicks(long ticks)
        {
            return (ticks * TimeSpan.TicksPerMillisecond) + 621355968000000000;
        }

        public static string TimezoneConvert(string timeString)
        {
            switch (timeString)
            {
                case "GMT":
                    return "+0:00";
                case "UTC":
                    return "+0:00";
                case "WET":
                    return DaylightSavingConverter(timeString);
                case "CET":
                    return DaylightSavingConverter(timeString);
                case "EET":
                    return DaylightSavingConverter(timeString);
                case "MSK":
                    return "+3:00";
                case "GST":
                    return "+4:00";
                case "TTFT":
                    return "+5:00";
                case "SLST":
                    return "+5:30";
                case "BST":
                    return "+6:00";
                case "WIT":
                    return "+7:00";
                case "AWST":
                    return DaylightSavingConverter(timeString);
                case "ACST":
                    return DaylightSavingConverter(timeString);
                case "JST":
                    return "+9:00";
                case "KST":
                    return "+9:00";
                case "AEST":
                    return DaylightSavingConverter(timeString);
                case "NCT":
                    return "+11:00";
                case "NZST":
                    return DaylightSavingConverter(timeString);
                case "CVT":
                    return "-1:00";
                case "UYST":
                    return "-2:00";
                case "PMST":
                    return "-3:00";
                case "NST":
                    return DaylightSavingConverter(timeString);
                case "AST":
                    return "-4:00";
                case "EST":
                    return DaylightSavingConverter(timeString);
                case "CST":
                    return DaylightSavingConverter(timeString);
                case "MST":
                    return DaylightSavingConverter(timeString);
                case "PST":
                    return DaylightSavingConverter(timeString);
                case "AKST":
                    return DaylightSavingConverter(timeString);
                case "HST":
                    return "-10:00";
                default:
                    throw new NotSupportedException($"The current time zone {timeString} is not supported");
            }
        }

        private static string DaylightSavingConverter(string zone)
        {
            string zoneID;
            switch (zone)
            {
                case "WET":
                    zoneID = "W. Europe Standard Time";
                    break;
                case "CET":
                    zoneID = "Central Europe Standard Time";
                    break;
                case "EET":
                    zoneID = "E. Europe Standard Time";
                    break;
                case "AWST":
                    zoneID = "W. Australia Standard Time";
                    break;
                case "ACST":
                    zoneID = "Cen. Australia Standard Time";
                    break;
                case "AEST":
                    zoneID = "E. Australia Standard Time";
                    break;
                case "NZST":
                    zoneID = "New Zealand Standard Time";
                    break;
                case "NST":
                    zoneID = "Newfoundland Standard Time";
                    break;
                case "EST":
                    zoneID = "Eastern Standard Time";
                    break;
                case "CST":
                    zoneID = "Central Standard Time";
                    break;
                case "MST":
                    zoneID = "US Mountain Standard Time";
                    break;
                case "PST":
                    zoneID = "Pacific Standard Time";
                    break;
                case "AKST":
                    zoneID = "Alaskan Standard Time";
                    break;
                default:
                    throw new NotSupportedException($"The current time zone {zone} is not supported");
            }

            //now with the zone ID, put the current time to see if it is day light time
            DateTime thisTime = DateTime.Now;
            // get Denmark Standard Time zone - not sure about that
            TimeZoneInfo tst = TimeZoneInfo.FindSystemTimeZoneById(zoneID);
            bool isDaylight = tst.IsDaylightSavingTime(thisTime);

            int offset;

            if (isDaylight)
            {
                offset = tst.BaseUtcOffset.Hours + 1;
            }
            else
            {
                offset = tst.BaseUtcOffset.Hours;
            }

            return offset > 0 ? $"+{offset}:00" : $"{offset}:00";
        }
    }
}
