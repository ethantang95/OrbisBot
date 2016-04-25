﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.OrbScript.Functions
{
    static class ScriptBasicFunctions
    {
        public static string Add(string a, string b)
        {
            var first = double.Parse(a);
            var second = double.Parse(b);
            return Math.Round(first + second).ToString();
        }

        public static string Subtract(string a, string b)
        {
            var first = double.Parse(a);
            var second = double.Parse(b);
            return Math.Round(first - second).ToString();
        }

        public static string Multiply(string a, string b)
        {
            var first = double.Parse(a);
            var second = double.Parse(b);
            return (first * second).ToString();
        }

        public static string Divide(string a, string b)
        {
            var first = double.Parse(a);
            var second = double.Parse(b);
            return (first / second).ToString();
        }

        public static string Max(string a, string b)
        {
            var first = double.Parse(a);
            var second = double.Parse(b);
            return first > second ? first.ToString() : second.ToString();
        }

        public static string Min(string a, string b)
        {
            var first = double.Parse(a);
            var second = double.Parse(b);
            return first < second ? first.ToString() : second.ToString();
        }

        public static string Equal(string a, string b)
        {
            return a.Equals(b, StringComparison.InvariantCultureIgnoreCase).ToString();
        }

        public static string Greater(string a, string b)
        {
            var first = double.Parse(a);
            var second = double.Parse(b);
            return (first > second).ToString();
        }

        public static string GreaterEqual(string a, string b)
        {
            var first = double.Parse(a);
            var second = double.Parse(b);
            return (first >= second).ToString();
        }

        public static string Less(string a, string b)
        {
            var first = double.Parse(a);
            var second = double.Parse(b);
            return (first < second).ToString();
        }

        public static string LessEqual(string a, string b)
        {
            var first = double.Parse(a);
            var second = double.Parse(b);
            return (first <= second).ToString();
        }

        public static string Not(string a)
        {
            var first = bool.Parse(a);
            return (!first).ToString();
        }

        public static string And(string a, string b)
        {
            var first = bool.Parse(a);
            var second = bool.Parse(b);
            return (first & second).ToString();
        }

        public static string Or(string a, string b)
        {
            var first = bool.Parse(a);
            var second = bool.Parse(b);
            return (first | second).ToString();
        }

        public static string Xor(string a, string b)
        {
            var first = bool.Parse(a);
            var second = bool.Parse(b);
            return (first ^ second).ToString();
        }

        public static string If(string eval, string trueReturn, string falseReturn)
        {
            var evalBool = bool.Parse(eval);
            return evalBool ? trueReturn : falseReturn;
        }

        public static string Loop(string term, string iterations)
        {
            var iter = int.Parse(iterations);
            var builder = new StringBuilder();
            for (int i = 0; i < iter; i++)
            {
                builder.Append(term);
            }
            return builder.ToString();
        }

        public static string Random(string lower, string upper)
        {
            var first = int.Parse(lower);
            var second = int.Parse(upper);

            return new Random().Next(first, second + 1).ToString();
        }
    }
}
