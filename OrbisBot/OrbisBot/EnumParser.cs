using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot
{
    class EnumParser
    {
        public static T ParseString<T>(string toParse, T defaultSelection)
        {
            return ParseString(toParse, false, defaultSelection);
        }

        public static T ParseString<T>(string toParse, bool throwOnFail, T defaultSelection)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), toParse);
            }
            catch (Exception e)
            {
                if (throwOnFail)
                {
                    throw e;
                }
                return defaultSelection;
            }
        }
    }
}
