using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OrbisBot.OrbScript
{
    static class OrbScriptUtils
    {
        public static string FindFunctionName(string s)
        {
            var detectedFunction = Regex.Match(s, @"(%[\w]*\()");

            if (detectedFunction.Groups.Count != 1)
            {
                throw new ArgumentException("Found no or more than 1 functions in the given signature");
            }

            return detectedFunction.Groups[0].Value.Replace("%", "").Replace("(", "");
        }
    }
}
