using NUnit.Framework;
using OrbisBot.OrbScript;
using OrbisBotUnitTest.OrbScriptTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBotUnitTest.OrbScriptTests
{
    [TestFixture]
    class OrbScriptEvaluatorTests
    {
        [Test]
        public void TestConstructor()
        {
            ConstructEvaluator(CreateVarExprList(), DeclarDictBuilder.BuildEmpty(), string.Empty);
        }

        [Test]
        public void TestEvaluateSimpleVariable()
        {
            var variables = CreateVarExprList("$var1");
        }

        private OrbScriptEvaluator ConstructEvaluator(List<string> variables, Dictionary<string, string> expressions, string response)
        {
            var extractorResult = new OrbScriptExtractorResults(variables, expressions, response);

            return new OrbScriptEvaluator(extractorResult);
        }

        private List<string> CreateVarExprList(params string[] varExprs)
        {
            return varExprs.ToList();
        }
    }

    class DeclarDictBuilder
    {
        Dictionary<string, string> _declarDict;

        DeclarDictBuilder(string key, string value)
        {
            _declarDict = new Dictionary<string, string>();
            _declarDict.Add(key, value);
        }

        public DeclarDictBuilder Add(string key, string value)
        {
            _declarDict.Add(key, value);
            return this;
        }

        public Dictionary<string, string> GetDictionary()
        {
            return _declarDict;
        }

        public static DeclarDictBuilder Create(string key, string value)
        {
            var builder = new DeclarDictBuilder(key, value);
            return builder;
        }

        public static Dictionary<string, string> BuildEmpty()
        {
            return new Dictionary<string, string>();
        }
    }
}
