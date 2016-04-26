using Discord;
using OrbisBot.OrbScript.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.OrbScript
{
    class OrbScriptEngine
    {
        private OrbScriptConfiger _config;
        private User _focusUser;
        private string _evalString;
        private Dictionary<string, string> _vars;
        private OrbScriptLexer _lexer;

        public OrbScriptEngine(OrbScriptConfiger config, User focusUser)
        {
            _config = config;
            _focusUser = focusUser;
            _vars = new Dictionary<string, string>();
        }

        public void SetArgs(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                _vars.Add($"param{i+1}", args[i]);
            }
            _vars.Add("user", _focusUser.Name);
        }

        public string EvaluateString(string s)
        {
            _evalString = s;
            _lexer = new OrbScriptLexer(s);
            var startWithVar = ExtractVariables();
            var builder = new StringBuilder();

            if (!startWithVar)
            {
                _evalString = _evalString.Substring(_lexer.Index - _lexer.Token.Length);
            }

            while (_evalString.Length != 0)
            {
                if (_evalString[0] == '$' || _evalString[0] == '~')
                {
                    _lexer = new OrbScriptLexer(_evalString);
                    var term = EvaluateTerm();
                    builder.Append(term + " ");
                    _evalString = _evalString.Substring(_lexer.Index - _lexer.Token.Length);
                }
                else
                {
                    var c = _evalString[0];
                    builder.Append(c);
                    _evalString = _evalString.Substring(1);
                }
            }
            return builder.ToString();
        }

        private bool ExtractVariables()
        {
            //args will have the format $<name>=(<eval>)
            while (_lexer.inspect("$"))
            {
                _lexer.consume("$");
                var varName = _lexer.consumeVar();
                if (_vars.ContainsKey(varName))
                {
                    var position = _evalString.IndexOf("$" + varName);
                    _evalString = _evalString.Substring(position);
                    return true;
                }
                _lexer.consume("=");
                var value = EvaluateTerm();
                _vars.Add(varName, value);
                _lexer.consume(";");
            }

            return false;
        }

        private string EvaluateTerm()
        {
            if (_lexer.inspect("$"))
            {
                var result = EvaluateVariable();
                return result;
            }
            else if (_lexer.inspect("~"))
            {
                var result = EvaluateFunction();
                return result;
            }

            throw new ArgumentException("Attempted to evaluate a term that is not a variable for a function");
        }

        private string EvaluateVariable()
        {
            //implicitly it can only be called by EvalTerm
            _lexer.consume("$");
            var name = _lexer.consumeVar();
            if (!_vars.ContainsKey(name))
            {
                throw new ArgumentException($"Cannot find the variable {name}");
            }
            return _vars[name];
        }

        private string EvaluateFunction()
        {
            //more bloody complicated...
            _lexer.consume("~");
            var varName = _lexer.consumeVar();
            var varParams = new List<string>();
            //all variable start with a bracket
            _lexer.consume("(");
            //now, it contains a variable... fk, this might be hard to parse
            //first case, if it's a call another function
            varParams.Add(EvaluateParam());
            while (_lexer.inspect(","))
            {
                _lexer.consume(",");
                varParams.Add(EvaluateParam());
            }
            _lexer.consume(")");
            return FunctionCallBranch(varName, varParams.ToArray());
        }

        private string EvaluateParam()
        {
            if (_lexer.inspect("$", "~"))
            {
                //recursive
                var term = EvaluateTerm();
                return term;
            }
            //if it's a string...
            else if (_lexer.inspect("{"))
            {
                //we kinda have to manually parse it
                //hopfully index is where we want it to be
                _lexer.consume("{");
                var builder = new StringBuilder();
                _evalString = _evalString.Substring(_lexer.Index - _lexer.Token.Length);
                while (_evalString[0] != '}')
                {    
                    var c = _evalString[0];
                    builder.Append(c);
                    _evalString = _evalString.Substring(1);   
                }
                _lexer = new OrbScriptLexer(_evalString);
                _lexer.consume("}");
                //we parsed out that string
                return builder.ToString();
            }
            else //can only be a single entree
            {
                var param = _lexer.consumeVar();
                return param;
            }
        }

        private string FunctionCallBranch(string name, string[] args)
        {
            switch (name)
            {
                case "Execute": CheckIfStandard(name);
                    return OrbisBotFunctions.Execute(args, _config.EventArgs, _config.Iterations);

                case "MentionUser": return ScriptFunctions.MentionUser(_focusUser);
                case "FindAndMentionUser": CheckIfStandard(name);
                    return ScriptFunctions.FindAndMentionUser(args[0], _config.UserList, _config.IgnoreList);
                case "FindUser": CheckIfStandard(name);
                    return ScriptFunctions.FindUser(args[0], _config.UserList, _config.IgnoreList);
                case "MentionEveryone": return ScriptFunctions.MentionEveryone(_config.RoleList);
                case "MentionGroup": CheckIfEvent(name);
                    return ScriptFunctions.MentionGroup(_config.UserList);

                case "Add": return ScriptBasicFunctions.Add(args[0], args[1]);
                case "Subtract": return ScriptBasicFunctions.Subtract(args[0], args[1]);
                case "Multiply": return ScriptBasicFunctions.Multiply(args[0], args[1]);
                case "Divide": return ScriptBasicFunctions.Divide(args[0], args[1]);
                case "Mod": return ScriptBasicFunctions.Mod(args[0], args[1]);
                case "Power": return ScriptBasicFunctions.Power(args[0], args[1]);
                case "Ln": return ScriptBasicFunctions.Ln(args[0]);
                case "LogBase": return ScriptBasicFunctions.LogBase(args[0], args[1]);
                case "Max": return ScriptBasicFunctions.Max(args[0], args[1]);
                case "Min": return ScriptBasicFunctions.Min(args[0], args[1]);
                case "Round": return ScriptBasicFunctions.Round(args[0], args[1]);
                case "Equal": return ScriptBasicFunctions.Equal(args[0], args[1]);
                case "Greater": return ScriptBasicFunctions.Greater(args[0], args[1]);
                case "GreaterEqual": return ScriptBasicFunctions.GreaterEqual(args[0], args[1]);
                case "Less": return ScriptBasicFunctions.Less(args[0], args[1]);
                case "LessEqual": return ScriptBasicFunctions.LessEqual(args[0], args[1]);
                case "Not": return ScriptBasicFunctions.Not(args[0]);
                case "And": return ScriptBasicFunctions.And(args[0], args[1]);
                case "Or": return ScriptBasicFunctions.Or(args[0], args[1]);
                case "Xor": return ScriptBasicFunctions.Xor(args[0], args[1]);
                case "If": return ScriptBasicFunctions.If(args[0], args[1], args[2]);
                //case "Loop": return ScriptBasicFunctions.LessEqual(args[0], args[1]);
                case "Random": return ScriptBasicFunctions.Random(args[0], args[1]);
                case "BoolToNum": return ScriptCastFunctions.BoolToNum(args[0]);
                case "NumToBool": return ScriptCastFunctions.NumToBool(args[0]);
                default: throw new ArgumentException($"Function {name} does not exist");
            }
        }

        private void CheckIfEvent(string name)
        {
            if (_config.BuildType != OrbScriptBuildType.Events)
            {
                throw new InvalidOperationException($"Cannot call method {name} for custom messages that are not events");
            }
        }

        private void CheckIfStandard(string name)
        {
            if (_config.BuildType != OrbScriptBuildType.Standard)
            {
                throw new InvalidOperationException($"Cannot call method {name} for custom messages that are not standard custom messages");
            }
        }

        private void CheckIfWelcome(string name)
        {
            if (_config.BuildType != OrbScriptBuildType.Welcome)
            {
                throw new InvalidOperationException($"Cannot call method {name} for custom messages that are not welcome messages");
            }
        }
    }
}
