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
                throw new ArgumentException("Found no or more than 1 function calls in the given signature");
            }

            return detectedFunction.Groups[0].Value.Replace("%", "").Replace("(", "");
        }

        public static string ExtractBracketContent(string toExtract, int position)
        {
            var bracketPositions = FindStartAndEndInnerContent(toExtract, position);
            var startPosition = bracketPositions.Item1;
            var endPosition = bracketPositions.Item2;

            if (endPosition == -1)
            {
                throw new FormatException("There is no closing bracket for this token, please check the command syntax");
            }

            //now we know where the start and the end of this substring is
            var toReturn = toExtract.Substring(startPosition, endPosition - startPosition + 1);
            toReturn = toReturn.Substring(1, toReturn.Length - 2).Trim(); //remove the brackets

            return toReturn;
        }

        private static Tuple<int, int> FindStartAndEndInnerContent(string content, int position)
        {
            var bracketsCount = 0;
            var startPosition = -1;
            var endPosition = -1;
            var started = false;
            for (int i = position; i < content.Length && (bracketsCount != 0 || !started); i++)
            {
                if (content[i] == '(')
                {
                    if (!started)
                    {
                        started = true;
                        startPosition = i;
                        bracketsCount++;
                    }
                    else
                    {
                        bracketsCount++;
                    }
                }
                else if (content[i] == ')')
                {
                    if (started) //I really do not understand why this would happen though
                    {
                        bracketsCount--;
                        if (bracketsCount == 0) //ehh, more readable
                        {
                            endPosition = i;
                        }
                    }
                }
            }

            if (endPosition == -1)
            {
                throw new FormatException($"There is no closing bracket for this content at token position character {position}, \"{content.Substring(startPosition, 10)}\", please check the command syntax");
            }

            return new Tuple<int, int>(startPosition, endPosition);
        }
    }
}
