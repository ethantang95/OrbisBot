using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.OrbScript
{
    public class OrbScriptLexer
    {
        public string Token { get; private set; }

        private enum Kind { SIMPLE, VAR, NUM, STRING, EOF }

        private enum State { START, CONTINUE_VAR, CONTINUE_NUM, CONTINUE_STRING }

        private int _index;
        private char[] _input;
        private Kind _kind;

        public OrbScriptLexer(string input)
        {
            _input = input.ToCharArray();
            _index = 0;
            advance();
        }

        public bool inspect(string s)
        {
            return Token.Equals(s);
        }

        public bool inspect(params string[] options)
        {
            foreach (var s in options)
            {
                if (inspect(s))
                {
                    return true;
                }
            }
            return false;
        }

        public bool inspectVar()
        {
            return _kind == Kind.VAR;
        }

        public bool inspectNum()
        {
            return _kind == Kind.NUM;
        }

        public bool inspectString()
        {
            return _kind == Kind.STRING;
        }

        public bool inspectEOF()
        {
            return _kind == Kind.EOF;
        }

        public string consume(string s)
        {
            if (Token.Equals(s))
            {
                advance();
                return s;
            }
            else
            {
                throw err($"expected: '{s}' got '{Token}'");
            }
        }

        public string consume(params string[] options)
        {
            foreach (var s in options)
            {
                if (inspect(s))
                {
                    return consume(s);
                }
            }
            throw err($"expected one of '{string.Join(",", options)}' but had '{Token}'");
        }

        public string consumeVar()
        {
            if (!inspectVar())
            {
                throw err($"expected: Var got '{Token}'");
            }
            string result = Token;
            advance();
            return result;
        }

        public string consumeNum()
        {
            if (!inspectNum())
            {
                throw err($"expected: Num got '{Token}'");
            }
            string result = Token;
            advance();
            return result;
        }

        public string consumeString()
        {
            if (!inspectString())
            {
                throw err($"expected: String, got '{Token}'");
            }
            string result = Token;
            advance();
            return result;

        }

        private bool isContinueID(char ch)
        {
            return (char.IsLetterOrDigit(ch) || ch == '_' || ch == '-');
        }

        private bool isContinueNum(char ch)
        {
            return (char.IsNumber(ch) || ch == '.');
        }

        private bool isContinueString(char ch)
        {
            return ch != '}';
        }

        private void advance()
        {
            StringBuilder stringBuilder = new StringBuilder();
            State state = State.START;
            _kind = Kind.SIMPLE;

            if (_index == _input.Length)
            {
                _kind = Kind.EOF;
            }

            while (_index < _input.Length)
            {
                char ch = _input[_index];
                ++_index;

                if (state == State.START)
                {
                    if (char.IsWhiteSpace(ch))
                    {
                        // We may have got to the end by ignoring whitespace
                        if (_index == _input.Length)
                        {
                            _kind = Kind.EOF;
                        }
                        continue;
                    }
                    else if (char.IsLetter(ch))
                    {
                        _kind = Kind.VAR;
                        state = State.CONTINUE_VAR;
                    }
                    else if (char.IsNumber(ch) || ch == '-' || ch == '+')
                    {
                        _kind = Kind.NUM;
                        state = State.CONTINUE_NUM;
                    }
                    else if (ch == '{')
                    {
                        _kind = Kind.STRING;
                        state = State.CONTINUE_STRING;
                    }
                }
                else if (state == State.CONTINUE_VAR)
                {
                    if (!isContinueID(ch))
                    {
                        --_index;
                        break;
                    }
                }
                else if (state == State.CONTINUE_NUM)
                {
                    if (!isContinueNum(ch))
                    {
                        --_index;
                        break;
                    }
                }
                else if (state == State.CONTINUE_STRING)
                {
                    if (!isContinueString(ch))
                    {
                        break;
                    }
                }
                stringBuilder.Append(ch);
                if (state == State.START)
                {
                    break;
                }
            }
            Token = stringBuilder.ToString();

            if (_kind == Kind.STRING)
            {
                //remove the curly braces
                Token = Token.Substring(1);
            }
        }

        private Exception err(string msg)
        {
            return new ArgumentException(msg);
        }
    }
}
