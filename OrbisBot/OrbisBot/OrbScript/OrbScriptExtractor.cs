using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.OrbScript
{
    public class OrbScriptExtractor
    {
        private List<string> _extractedVariablesExpressions;
        private Dictionary<string, string> _extractedDeclarationsWithSymbols;
        private string _tokenizedResponse;

        private string _originalResponse;
        private string _replyOnlyResponse;

        public OrbScriptExtractor(string response)
        {
            if (response == null)
            {
                throw new OrbScriptException("The passed message to evaluate is null");
            }

            _originalResponse = response;
            _extractedVariablesExpressions = new List<string>();
            _extractedDeclarationsWithSymbols = new Dictionary<string, string>();
        }

        public OrbScriptExtractorResults Extract()
        {
            ExtractVariablesAndFindResponse();
            ExtractResponseFunctions();
            return new OrbScriptExtractorResults(_extractedVariablesExpressions, _extractedDeclarationsWithSymbols, _tokenizedResponse);
        }

        private void ExtractVariablesAndFindResponse()
        {
            var splittedVariables = _originalResponse.Split(SymbolConstants.VAR_DECLARE_SEPARATOR_CHAR);
            //All our candidates should start with at least $var=$ or $var=~ or $var=@

            for (int i = 0; i < splittedVariables.Length; i++)
            {
                var variableCandidate = splittedVariables[i].Trim();

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
                    _extractedVariablesExpressions.Add(variableCandidate);
                }
            }
        }

        private bool InspectVariableSyntax(string variableCandidate)
        {
            var lexer = new OrbScriptLexer(variableCandidate);
            var valid = true;

            valid &= InspectAndConsumeSymbols(lexer, SymbolConstants.VAR_START);
            valid &= InspectAndConsumeVariable(lexer);
            valid &= InspectAndConsumeSymbols(lexer, SymbolConstants.VAR_ASSIGN);
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

        private void ExtractResponseFunctions()
        {
            var foundDeclaration = new List<string>();

            //we will just do linear scan and stop when we find a ~
            for (int i = 0; i < _replyOnlyResponse.Length; i++)
            {
                if (SymbolConstants.ALL_STARTS_CHAR.Contains(_replyOnlyResponse[i]))
                {
                    var partialString = _replyOnlyResponse.Substring(i);
                    var lexer = new OrbScriptLexer(partialString);
                    if (_replyOnlyResponse[i] == SymbolConstants.FUNC_START_CHAR)
                    {
                        if (IsFunctionDeclaration(lexer))
                        {
                            var expression = ExtractFunction(partialString);
                            foundDeclaration.Add(expression);
                            var advanceAmount = expression.Length;
                            i += advanceAmount - 1;
                        }
                    }
                    else if (_replyOnlyResponse[i] == SymbolConstants.VAR_START_CHAR)
                    {
                        if (IsVariableDeclaration(_replyOnlyResponse, i))
                        {
                            var declaration = $"{lexer.consume(SymbolConstants.VAR_START)}{lexer.consumeVar()}";
                            foundDeclaration.Add(declaration);
                            var advanceAmount = declaration.Length;
                            i += advanceAmount - 1;
                        }
                    }
                    //we will not inspect a string inside a reply... because first, it makes
                    //no sense to include a string declaration, and second, discord uses @ for mentions
                }
            }

            ReplaceExtractedFunctions(foundDeclaration);
        }

        private string ExtractFunction(string functionString)
        {
            var isInString = false;
            var functionStarted = false;
            var bracketCount = 0;

            for (int i = 0; i < functionString.Length; i++)
            {
                var ch = functionString[i];

                if (!isInString)
                {
                    if (ch == SymbolConstants.FUNC_BRACKET_START_CHAR)
                    {
                        bracketCount++;
                        functionStarted = true;
                    }
                    else if (ch == SymbolConstants.FUNC_BRACKET_END_CHAR)
                    {
                        bracketCount--;
                    }
                }

                if (ch == OrbScriptLexer.STRING_BRACKET_START_CHAR)
                {
                    isInString = true;
                }
                else if (ch == OrbScriptLexer.STRING_BRACKET_END_CHAR)
                {
                    isInString = false;
                }

                if (bracketCount == 0 && functionStarted)
                {
                    var extractedFunction = functionString.Substring(0, i + 1);
                    return extractedFunction;
                }
            }

            throw new OrbScriptException($"There does not appear to be a closing bracket in your function expression: {functionString}");
        }

        private void ReplaceExtractedFunctions(List<string> foundFunctions)
        {
            var responseCopy = _replyOnlyResponse;
            foreach (var foundfunction in foundFunctions)
            {
                var placeholderSymbol = $"$symbol{_extractedDeclarationsWithSymbols.Count}$";
                _extractedDeclarationsWithSymbols.Add(placeholderSymbol, foundfunction);

                responseCopy = responseCopy.Replace(foundfunction, placeholderSymbol);
            }

            _tokenizedResponse = responseCopy;
        }

        private bool IsVariableDeclaration(string str, int position)
        {
            //last character
            if (position == str.Length - 1)
            {
                return false;
            }
            return char.IsLetter(str[position + 1]);
        }

        private bool IsFunctionDeclaration(OrbScriptLexer lexer)
        {
            //we will use the lexer to validate up until there is a bracket
            var stillFunctionDeclaration = true;

            stillFunctionDeclaration &= InspectAndConsumeSymbols(lexer, SymbolConstants.FUNC_START);
            stillFunctionDeclaration &= InspectAndConsumeVariable(lexer);
            stillFunctionDeclaration &= InspectAndConsumeSymbols(lexer, SymbolConstants.FUNC_BRACKET_START);

            return stillFunctionDeclaration;
        }
    }

    public class OrbScriptExtractorResults
    {
        public List<string> ExtractedVariablesExpressions { get; private set; }
        public Dictionary<string, string> ExtractedDeclarationsWithSymbols { get; private set; }
        public string TokenizedResponse { get; private set; }

        public OrbScriptExtractorResults(List<string> extractedVariables, Dictionary<string, string> extractedDeclarations, string response)
        {
            ExtractedVariablesExpressions = extractedVariables;
            ExtractedDeclarationsWithSymbols = extractedDeclarations;
            TokenizedResponse = response;
        }
    }
}
