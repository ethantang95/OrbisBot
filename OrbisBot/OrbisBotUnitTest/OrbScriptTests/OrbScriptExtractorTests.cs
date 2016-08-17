using NUnit.Framework;
using OrbisBot.OrbScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBotUnitTest.OrbScriptTests
{
    [TestFixture]
    public class OrbScriptExtractorTests
    {
        [Test]
        public void TestExtractorConstructor()
        {
            Extract("Foo");
        }

        [Test]
        public void TestSimpleExtractingVariableWithFunction()
        {
            var extractor = Extract("$var=~function();");
            Assert.Contains("$var=~function()", extractor.ExtractedVariablesExpressions);
        }

        [Test]
        public void TestSimpleExtractingMultiVariablesWithFunction()
        {
            var extractor = Extract("$var=~function(); $var2=~function2();");
            Assert.Contains("$var=~function()", extractor.ExtractedVariablesExpressions);
            Assert.Contains("$var2=~function2()", extractor.ExtractedVariablesExpressions);
        }

        [Test]
        public void TestSimpleExtractingVariableWithVariable()
        {
            var extractor = Extract("$var=$var2;");
            Assert.Contains("$var=$var2", extractor.ExtractedVariablesExpressions);
        }

        [Test]
        public void TestSimpleExtractingVariableWithMultiVariable()
        {
            var extractor = Extract("$var=$var2; $var3=$var4;");
            Assert.Contains("$var=$var2", extractor.ExtractedVariablesExpressions);
            Assert.Contains("$var3=$var4", extractor.ExtractedVariablesExpressions);
        }

        [Test]
        public void TestSimpleExtractingVariableWithString()
        {
            var extractor = Extract("$var=@{a string};");
            Assert.Contains("$var=@{a string}", extractor.ExtractedVariablesExpressions);
        }

        [Test]
        public void TestSimpleExtractingVariableWithMultiString()
        {
            var extractor = Extract("$var=@{a string}; $var2=@{a string 2};");
            Assert.Contains("$var=@{a string}", extractor.ExtractedVariablesExpressions);
            Assert.Contains("$var2=@{a string 2}", extractor.ExtractedVariablesExpressions);
        } 

        [Test]
        public void TestComplexExtracting()
        {
            var extractor = Extract("$var=~function(); $var2=$var4; $var3=@{a string};");
            Assert.Contains("$var=~function()", extractor.ExtractedVariablesExpressions);
            Assert.Contains("$var2=$var4", extractor.ExtractedVariablesExpressions);
            Assert.Contains("$var3=@{a string}", extractor.ExtractedVariablesExpressions);
        }

        [Test]
        public void TestBadVariableSyntax()
        {
            Assert.Throws<OrbScriptException>(() => Extract("$vars=!;"));
            
        }

        [Test]
        public void TestNoDeclarationExtraction()
        {
            var extractor = Extract("This is a message");
            Assert.AreEqual(0, extractor.ExtractedVariablesExpressions.Count);
            Assert.AreEqual(0, extractor.ExtractedDeclarationsWithSymbols.Count);
            Assert.AreEqual("This is a message", extractor.TokenizedResponse);
        }

        [Test]
        public void TestExtractingLeadingVariableDeclarationResponse()
        {
            var extractor = Extract("$name is foo");
            Assert.AreEqual(0, extractor.ExtractedVariablesExpressions.Count);
            var index0Symbol = GetFunctionSymbolForIndex(0);

            AssertExtractorHasDeclarationForSymbol("$name", index0Symbol, extractor);
            Assert.AreEqual($"{index0Symbol} is foo", extractor.TokenizedResponse);
        }

        [Test]
        public void TestExtractingVariableDeclarationAndResponse()
        {
            var extractor = Extract("$var1=~function1(); This is a message");
            Assert.Contains("$var1=~function1()", extractor.ExtractedVariablesExpressions);
            Assert.AreEqual("This is a message", extractor.TokenizedResponse);
        }

        [Test]
        public void TestExtractingFunctionDeclarations()
        {
            var func1Declaration = "~function1($var1, var2)";
            var extractor = Extract($"This is a {func1Declaration}");
            var index0Symbol = GetFunctionSymbolForIndex(0);

            AssertExtractorHasDeclarationForSymbol(func1Declaration, index0Symbol, extractor);
            Assert.AreEqual($"This is a {index0Symbol}", extractor.TokenizedResponse);
        }

        [Test]
        public void TestExtractingFunctionDeclarationWithNestedFunctions()
        {
            var func1declaration = "~function1(~function2($param2))";
            var extractor = Extract($"This is a {func1declaration}");
            var index0Symbol = GetFunctionSymbolForIndex(0);

            AssertExtractorHasDeclarationForSymbol(func1declaration, index0Symbol, extractor);
            Assert.AreEqual($"This is a {index0Symbol}", extractor.TokenizedResponse);
        }

        [Test]
        public void TestExtractingFunctionDeclarationWithString()
        {
            var func1Declaration = "~function1(@{I am happy :)})";
            var extractor = Extract($"This is a {func1Declaration}");
            var index0Symbol = GetFunctionSymbolForIndex(0);

            AssertExtractorHasDeclarationForSymbol(func1Declaration, index0Symbol, extractor);
            Assert.AreEqual($"This is a {index0Symbol}", extractor.TokenizedResponse);
        }

        [Test]
        public void TestLessClosingBracketDeclaration()
        {
            var func1Declaration = "~function1(~function2()";
            Assert.Throws<OrbScriptException>(() => Extract($"This is a {func1Declaration}"));

        }

        [Test]
        public void TestDollarSignSymbol()
        {
            var extractor = Extract($"This is $ something");
            Assert.AreEqual(0, extractor.ExtractedDeclarationsWithSymbols.Count);
            Assert.AreEqual("This is $ something", extractor.TokenizedResponse);
        }

        [Test]
        public void TestMultiDollarSignSymbol()
        {
            var extractor = Extract($"This is $$$");
            Assert.AreEqual(0, extractor.ExtractedDeclarationsWithSymbols.Count);
            Assert.AreEqual("This is $$$", extractor.TokenizedResponse);
        }

        [Test]
        public void TestDollarSignWithExpression()
        {
            var expression1 = "$value";
            var extractor = Extract($"This is ${expression1}");
            var index0Symbol = GetFunctionSymbolForIndex(0);

            AssertExtractorHasDeclarationForSymbol(expression1, index0Symbol, extractor);
            Assert.AreEqual($"This is ${index0Symbol}", extractor.TokenizedResponse);

        }

        [Test]
        public void TestDollarSignSymbolWithNumbers()
        {
            var extractor = Extract($"This is $1.11");
            Assert.AreEqual(0, extractor.ExtractedDeclarationsWithSymbols.Count);
            Assert.AreEqual("This is $1.11", extractor.TokenizedResponse);
        }

        [Test]
        public void TestDollarSignEndOfLine()
        {
            var extractor = Extract($"This is $");
            Assert.AreEqual(0, extractor.ExtractedDeclarationsWithSymbols.Count);
            Assert.AreEqual("This is $", extractor.TokenizedResponse);
        }

        [Test]
        public void TestTildaSignSymbol()
        {
            var extractor = Extract($"This is ~ something");
            Assert.AreEqual(0, extractor.ExtractedDeclarationsWithSymbols.Count);
            Assert.AreEqual("This is ~ something", extractor.TokenizedResponse);
        }

        [Test]
        public void TestTildaSignSymbolRepeat()
        {
            var extractor = Extract($"This is ~~");
            Assert.AreEqual(0, extractor.ExtractedDeclarationsWithSymbols.Count);
            Assert.AreEqual("This is ~~", extractor.TokenizedResponse);
        }

        [Test]
        public void TestTildaSignWithExpression()
        {
            var expression1 = "$value";
            var extractor = Extract($"This is ~{expression1}");
            var index0Symbol = GetFunctionSymbolForIndex(0);

            AssertExtractorHasDeclarationForSymbol(expression1, index0Symbol, extractor);
            Assert.AreEqual($"This is ~{index0Symbol}", extractor.TokenizedResponse);

        }

        [Test]
        public void TestTildaSignSymbolEndOfLine()
        {
            var extractor = Extract($"This is ~");
            Assert.AreEqual(0, extractor.ExtractedDeclarationsWithSymbols.Count);
            Assert.AreEqual("This is ~", extractor.TokenizedResponse);
        }

        [Test]
        public void TestEmpty()
        {
            var extractor = Extract(string.Empty);
            Assert.AreEqual(0, extractor.ExtractedDeclarationsWithSymbols.Count);
            Assert.AreEqual(0, extractor.ExtractedVariablesExpressions.Count);
            Assert.AreEqual(string.Empty, extractor.TokenizedResponse);
        }

        [Test]
        public void TestNull()
        {
            Assert.Catch<OrbScriptException>(() => Extract(null));
        }

        private void AssertExtractorHasDeclarationForSymbol(string declaration, string symbol, OrbScriptExtractorResults extractor)
        {
            var keyValue = new KeyValuePair<string, string>(symbol, declaration);
            Assert.Contains(keyValue, extractor.ExtractedDeclarationsWithSymbols);
        }

        private OrbScriptExtractorResults Extract(string toExtract)
        {
            var extractor = new OrbScriptExtractor(toExtract);
            return extractor.Extract();
        }

        private string GetFunctionSymbolForIndex(int i)
        {
            return $"$symbol{i}$";
        }
    }
}
