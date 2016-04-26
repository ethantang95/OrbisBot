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
    }
}
