using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.OrbScript
{
    class OrbScriptLexer
    {
        public int Index { get; private set; }
        public string Token { get; private set; }

        private enum Kind
        {
            SIMPLE,
            VAR,
            NUM,
            EOF,
        }

        private enum State
        {
            START,
            CONTINUE_VAR,
            CONTINUE_NUM
        }

        private char[] input;
        private Kind kind;

        public OrbScriptLexer(string input)
        {
            this.input = input.ToCharArray();
            Index = 0;
            advance();
        }

        private bool isContinueID(char ch)
        {
            return (char.IsLetterOrDigit(ch) || ch == '_' || ch == '-');
        }

        private bool isContinueNum(char ch)
        {
            return (char.IsNumber(ch) || ch == '.');
        }

        public void advance()
        {
            StringBuilder stringBuilder = new StringBuilder();
            State state = State.START;
            kind = Kind.SIMPLE;

            if (Index == input.Length)
            {
                kind = Kind.EOF;
            }

            while (Index < input.Length)
            {
                char ch = input[Index];
                ++Index;

                if (state == State.START)
                {
                    if (char.IsWhiteSpace(ch))
                    {
                        // We may have got to the end by ignoring whitespace
                        if (Index == input.Length)
                        {
                            kind = Kind.EOF;
                        }
                        continue;
                    }
                    else if (char.IsLetter(ch))
                    {
                        kind = Kind.VAR;
                        state = State.CONTINUE_VAR;
                    }
                    else if (char.IsNumber(ch) || ch == '-' || ch == '+')
                    {
                        kind = Kind.NUM;
                        state = State.CONTINUE_NUM;
                    }
                }
                else if (state == State.CONTINUE_VAR)
                {
                    if (!isContinueID(ch))
                    {
                        --Index;
                        break;
                    }
                }
                else if (state == State.CONTINUE_NUM)
                {
                    if (!isContinueNum(ch))
                    {
                        --Index;
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
        }

        /**
         * Checks to see whether String s is equal to the current token. Returns
         * true if they are equal, otherwise it will return false.
         * 
         * @param s
         * @return true if the current token equals parameter s
         */
        public bool inspect(string s)
        {
            return Token.Equals(s);
        }

        /**
         * Takes an array of strings and checks if the one of the elements inside
         * the array are equal to the current token.
         * 
         * @param options
         * @return true if any of the elements inside the array matches the token,
         *         otherwise it will return false.
         */
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

        /**
         * Checks to see if the current token is an ID i.e. the pin name.
         * 
         * @return true if the current token is an ID, otherwise it will return
         *         false.
         */
        public bool inspectVar()
        {
            return kind == Kind.VAR;
        }

        public bool inspectNum()
        {
            return kind == Kind.NUM;
        }

        /**
         * Checks to see if the lexer has reached the end of the file.
         * 
         * @return returns true if it is the end of file, otherwise it will return
         *         false.
         */
        public bool inspectEOF()
        {
            return kind == Kind.EOF;
        }

        /**
         * If String s matches the current token then the token is consumed,
         * parameter s is returned, then the lexer advances to the next token.
         * 
         * @param s
         * @return returns parameter s if parameter s equals the current token,
         *         otherwise returns null and outputs an error
         */
        public string consume(char c)
        {
            return consume(c.ToString());
        }
        public string consume(string s)
        {
            if (Token.Equals(s))
            {
                advance();
                return s;
            }
            else {
                err("expected: '" + s + "' got '" + Token + "'");
                return null; // dead code
            }
        }

        /**
         * Takes an array of strings. If any of the elements in the array matches
         * the token, the token is consumed, and the lexer advances to the next
         * token
         * 
         * @param options
         * @return the element matching the current token from parameter options,
         *         otherwise returns null and outputs an error
         */
        public string consume(params string[] options)
        {
            foreach (var s in options)
            {
                if (inspect(s))
                {
                    return consume(s);
                }
            }
            err("expected one of '" + options.ToString() + "' but had '"
                + Token + "'");
            return null;
        }

        /**
         * Consumes the ID and advances to the next token
         * 
         * @return string of ID is returned, otherwise returns null and outputs an
         *         error
         */
        public string consumeVar()
        {
            if (!inspectVar()) err("expected: Var got '" + Token + "'");
            string result = Token;
            advance();
            return result;
        }

        public string consumeNum()
        {
            if (!inspectNum()) err("expected: Num got '" + Token + "'");
            string result = Token;
            advance();
            return result;
        }

        /**
         * Consumes the end of file, otherwise an error is thrown.
         */
        public void consumeEOF()
        {
            if (!inspectEOF()) err("expected: EOF");
            advance();
        }

        /**
         * @return Returns the current value of the token.
         */
        public string debugState()
        {
            return Token;
        }

        protected void err(string msg)
        {
            throw new ArgumentException(msg);
        }
    }
}
