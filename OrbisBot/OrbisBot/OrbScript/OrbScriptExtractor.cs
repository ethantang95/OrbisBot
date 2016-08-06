using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.OrbScript
{
    public class OrbScriptExtractor
    {
        public List<string> ExtractedVariablesExpressions { get; private set; }
        public Dictionary<string, string> ExtractedFunctionsWithSymbols { get; private set; }
        public string TokenizedResponse { get; private set; }

        private string _originalResponse;
        private string _replyOnlyResponse;

        public OrbScriptExtractor(string response)
        {
            _originalResponse = response;
            ExtractedVariablesExpressions = new List<string>();
        }

        public void StartExtraction()
        {
            TryExtractVariablesAndFindResponse();
            TryExtractResponseFunctions();
            ExtractedFunctionsWithSymbols = new Dictionary<string, string>();
        }

        private void TryExtractVariablesAndFindResponse()
        {
            var splittedVariables = _originalResponse.Split(';');
            //All our candidates should start with at least $var=$ or $var=~ or $var=@

            for (int i = 0; i < splittedVariables.Length; i++)
            {
                var variableCandidate = splittedVariables[i];

                if (!InspectVariableSyntax(variableCandidate))
                {
                    if (i == splittedVariables.Length - 1)
                    {
                        //we don't expect the last string to be a variable, infact, it's the response
                        _replyOnlyResponse = variableCandidate;
                    }
                    else
                    {
                        throw new OrbScriptException($"There is a syntax error within your variable declaration with: {variableCandidate}. The expected syntax follows the form of $variable=$another_variable or $variable=~function() or $variable=$@{{a string}}");
                    }
                }
                else
                {
                    ExtractedVariablesExpressions.Add(variableCandidate);
                }
            }
        }

        private bool InspectVariableSyntax(string variableCandidate)
        {
            var lexer = new OrbScriptLexer(variableCandidate);
            var valid = true;

            valid &= InspectAndConsumeSymbols(lexer, SymbolConstants.VAR_START);
            valid &= InspectAndConsumeVariable(lexer);
            valid &= InspectAndConsumeSymbols(lexer, SymbolConstants.VAR_START);
            valid &= InspectAndConsumeSymbols(lexer, SymbolConstants.ALL_STARTS);

            return valid;
        }

        private bool InspectAndConsumeSymbols(OrbScriptLexer lexer, params string[] toInspect)
        {
            if (lexer.inspect(toInspect))
            {
                lexer.consume(toInspect);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool InspectAndConsumeVariable(OrbScriptLexer lexer)
        {
            if (lexer.inspectVar())
            {
                lexer.consumeVar();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void TryExtractResponseFunctions()
        {
            var foundFunctions = new List<string>();

            //we will just do linear scan and stop when we find a ~
            for (int i = 0; i < _replyOnlyResponse.Length; i++)
            {
                if (_replyOnlyResponse[i] == '~')
                {
                    var partialString = _replyOnlyResponse.Substring(i);
                    var lexer = new OrbScriptLexer(partialString);
                    //we will use the lexer to validate up until there is a bracket
                    var stillFunctionDeclaration = true;

                    stillFunctionDeclaration &= InspectAndConsumeSymbols(lexer, SymbolConstants.FUNC_START);
                    stillFunctionDeclaration &= InspectAndConsumeVariable(lexer);
                    stillFunctionDeclaration &= InspectAndConsumeSymbols(lexer, SymbolConstants.FUNC_BRACKET_START);

                    if (stillFunctionDeclaration)
                    {
                        var expression = ExtractFunction(partialString);
                        foundFunctions.Add(expression);
                        var advanceAmount = expression.Length;
                        i += advanceAmount - 1;
                    }
                }
            }
        }

        private string ExtractFunction(string functionString)
        {
            var isInString = false;
            var functionStarted = false;
            var bracketCount = 0;

            for (int i = 0; i < functionString.Length; i++)
            {
                var ch = functionString[i];
                if (bracketCount == 0 && functionStarted)
                {
                    var extractedFunction = functionString.Substring(0, i + 1);
                }
                else if (bracketCount < 0)
                {
                    throw new OrbScriptException($"There appear to be more closing brackets than open brackets in your expression {functionString}");
                }

                if (!isInString)
                {
                    if (ch == '(')
                    {
                        bracketCount++;
                        functionStarted = true;
                    }
                    else if (ch == ')')
                    {
                        bracketCount--;
                    }
                }

                if (ch == '{')
                {
                    isInString = true;
                }
                else if (ch == '}')
                {
                    isInString = false;
                }
            }

            throw new OrbScriptException($"There does not appear to be a closing bracket in your function expression: {functionString}");
        }

        private void ReplaceExtractedFunctions(List<string> foundFunctions)
        {
            var responseCopy = _replyOnlyResponse;
            foreach (var foundfunction in foundFunctions)
            {
                var placeholderSymbol = $"$symbol{ExtractedFunctionsWithSymbols.Count}$";
                ExtractedFunctionsWithSymbols.Add(placeholderSymbol, foundfunction);

                responseCopy = responseCopy.Replace(foundfunction, placeholderSymbol);
            }

            TokenizedResponse = responseCopy;
        }
    }
}
