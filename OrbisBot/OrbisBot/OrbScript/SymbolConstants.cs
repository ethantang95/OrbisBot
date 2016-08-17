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
        public const string STRING_START = "@"; //not sure if we want to use this symbol
        public const string VAR_ASSIGN = "=";
        public const string FUNC_BRACKET_START = "(";
        public const string FUNC_BRACKET_END = ")";
        public const string PARAM_SEPARATOR = ",";
        public const string VAR_DECLARE_SEPARATOR = ";";

        public const char VAR_START_CHAR = '$';
        public const char FUNC_START_CHAR = '~';
        public const char STRING_START_CHAR = '@';
        public const char VAR_ASSIGN_CHAR = '=';
        public const char FUNC_BRACKET_START_CHAR = '(';
        public const char FUNC_BRACKET_END_CHAR = ')';
        public const char PARAM_SEPARATOR_CHAR = ',';
        public const char VAR_DECLARE_SEPARATOR_CHAR = ';';

        public static readonly string[] ALL_STARTS = { VAR_START, FUNC_START, STRING_START };
        public static readonly char[] ALL_STARTS_CHAR = { VAR_START_CHAR, FUNC_START_CHAR, STRING_START_CHAR };
    }
}
