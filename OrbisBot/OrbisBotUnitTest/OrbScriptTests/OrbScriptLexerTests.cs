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
    public class OrbScriptLexerTests
    {
        [Test]
        public void TestLexerConstructor()
        {
            GetLexer("foo");
        }

        [Test]
        public void TestLexerInspectingSimple()
        {
            var lexer = GetLexer("!");
            Assert.True(lexer.inspect("!"));
        }

        [Test]
        public void TestLexerRejectSimple()
        {
            var lexer = GetLexer("!");
            Assert.False(lexer.inspect("?"));
        }

        [Test]
        public void TestLexerConsumeSimple()
        {
            var lexer = GetLexer("!");
            Assert.AreEqual("!", lexer.consume("!"));
        }

        [Test]
        public void TestLexerThrowSimple()
        {
            var lexer = GetLexer("!");
            Assert.Throws<ArgumentException>(() => lexer.consume("?"));
        }

        [Test]
        public void TestLexerInspectMultiSimple()
        {
            var lexer = GetLexer("!");
            Assert.True(lexer.inspect("?", "!"));
        }

        [Test]
        public void TestLexerRejectMultiSimple()
        {
            var lexer = GetLexer("!");
            Assert.False(lexer.inspect("@", "#"));
        }

        [Test]
        public void TestLexerConsumeMultiSimple()
        {
            var lexer = GetLexer("!");
            Assert.AreEqual("!", lexer.consume("?", "!"));
        }

        [Test]
        public void TestLexerThrowMultiSimple()
        {
            var lexer = GetLexer("!");
            Assert.Throws<ArgumentException>(() => lexer.consume("@", "#"));
        }

        [Test]
        public void TestLexerInspectVar()
        {
            var lexer = GetLexer("name");
            Assert.True(lexer.inspectVar());
            var lexer2 = GetLexer("name_underscored");
            Assert.True(lexer2.inspectVar());
            var lexer3 = GetLexer("name-dashed");
            Assert.True(lexer3.inspectVar());
        }

        [Test]
        public void TestLexerRejectVar()
        {
            var lexer = GetLexer("!");
            Assert.False(lexer.inspectVar());
        }

        [Test]
        public void TestLexerConsumeVar()
        {
            var lexer = GetLexer("name");
            Assert.AreEqual("name", lexer.consumeVar());
        }

        [Test]
        public void TestLexerThrowVar()
        {
            var lexer = GetLexer("!");
            Assert.Throws<ArgumentException>(() => lexer.consumeVar());
        }

        [Test]
        public void TestLexerInspectNumber()
        {
            var lexer = GetLexer("12");
            Assert.True(lexer.inspectNum());
            var lexer2 = GetLexer("12.12");
            Assert.True(lexer2.inspectNum());
            var lexer3 = GetLexer("+12");
            Assert.True(lexer3.inspectNum());
            var lexer4 = GetLexer("-12");
            Assert.True(lexer4.inspectNum());
        }

        [Test]
        public void TestLexerRejectNumber()
        {
            var lexer = GetLexer("asdf");
            Assert.False(lexer.inspectNum());
        }

        [Test]
        public void TestLexerConsumeNumber()
        {
            var lexer = GetLexer("12");
            Assert.AreEqual("12", lexer.consumeNum());
        }

        [Test]
        public void TestLexerThrowNumber()
        {
            var lexer = GetLexer("asdf");
            Assert.Throws<ArgumentException>(() => lexer.consumeNum());
        }

        [Test]
        public void TestLexerInspectString()
        {
            var lexer = GetLexer("{this is a string}");
            Assert.True(lexer.inspectString());
        }

        [Test]
        public void TestLexerRejectString()
        {
            var lexer = GetLexer("this is not a string");
            Assert.False(lexer.inspectString());
        }

        [Test]
        public void TestLexerConsumeString()
        {
            var lexer = GetLexer("{this is a string}");
            Assert.AreEqual("this is a string", lexer.consumeString());
        }

        [Test]
        public void TestLexerThrowString()
        {
            var lexer = GetLexer("this is not a string");
            Assert.Throws<ArgumentException>(() => lexer.consumeString());
        }

        [Test]
        public void TestLexerInspectEOF()
        {
            var lexer = GetLexer("");
            Assert.True(lexer.inspectEOF());
        }

        [Test]
        public void TestLexerRejectEOF()
        {
            var lexer = GetLexer("!");
            Assert.False(lexer.inspectEOF());
        }

        [Test]
        public void TestLexerConsumeAdvance()
        {
            var lexer = GetLexer("12 name @{this is a string}@ name 12 ");
            Assert.AreEqual("12", lexer.consumeNum());
            Assert.AreEqual("name", lexer.consumeVar());
            Assert.AreEqual("@", lexer.consume("@"));
            Assert.AreEqual("this is a string", lexer.consumeString());
            Assert.AreEqual("@", lexer.consume("@"));
            Assert.AreEqual("name", lexer.consumeVar());
            Assert.AreEqual("12", lexer.consumeNum());
            Assert.True(lexer.inspectEOF());            
        }

        private OrbScriptLexer GetLexer(string toLex)
        {
            return new OrbScriptLexer(toLex);
        }
    }
}
