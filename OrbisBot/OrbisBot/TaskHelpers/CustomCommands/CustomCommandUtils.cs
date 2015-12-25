using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.TaskHelpers.CustomCommands
{
    class CustomCommandUtils
    {
        //position being the location where we will find the token indicated by %
        public static string ExtractInnerContent(string toExtract, int position)
        {
            var bracketPositions = FindStartAndEndInnerContent(toExtract, position);
            var startPosition = bracketPositions.Item1;
            var endPosition = bracketPositions.Item2;

            if (endPosition == -1) {
                throw new FormatException("There is no closing bracket for this token, please check the command syntax");
            }

            //now we know where the start and the end of this substring is
            var toReturn = toExtract.Substring(startPosition, endPosition - startPosition + 1);
            toReturn = toReturn.Substring(1, toReturn.Length - 2).Trim(); //remove the brackets

            return toReturn;
        }

        public static bool ContainsTokens(string toDetermine)
        {
            //we are assuming that toDetermine only contains the inner content of the string, as extracted by ExtractInnerContent
            var hasToken = false;
            hasToken |= toDetermine.IndexOf("%r") > -1;

            //disabled but there are plans to support these commands in the future 
            //hasToken |= toDetermine.IndexOf("%e") > -1;
            //hasToken |= toDetermine.IndexOf("%?") > -1;
            //hasToken |= toDetermine.IndexOf("%v") > -1;

            return hasToken;
        }

        public static string ReplaceTokenWithValue(string toReplace, int tokenPosition, string value)
        {
            //the token position represents the start of content to replace
            var bracketPositions = FindStartAndEndInnerContent(toReplace, tokenPosition);
            //all I need is the end position of the bracket to extract the substring
            var endPosition = bracketPositions.Item2;
            var beginningString = toReplace.Substring(0, tokenPosition);
            var endString = toReplace.Substring(endPosition + 1); //substring is inclusive of the position
            return beginningString + value + endString;
        }

        public static string GetRandomValueForToken(string innerContent)
        {
            //format would be a:b where as a and b are integers
            var limits = innerContent.Split(':').Select(Int32.Parse).ToArray();
            var random = new Random();
            var value = random.Next(limits[0], limits[1] + 1);
            return value.ToString();
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
