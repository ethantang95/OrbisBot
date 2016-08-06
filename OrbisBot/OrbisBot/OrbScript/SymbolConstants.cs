using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.OrbScript
{
    class SymbolConstants
    {
        public const string VAR_START = "$";
        public const string FUNC_START = "~";
        public const string STRING_START = "@";
        public const string VAR_ASSIGN = "=";
        public const string FUNC_BRACKET_START = "(";
        public const string FUNC_BRACKET_END = ")";
        public const string PARAM_SEPARATOR = ",";

        public static readonly string[] ALL_STARTS = { VAR_START, FUNC_START, STRING_START };
    }
}
