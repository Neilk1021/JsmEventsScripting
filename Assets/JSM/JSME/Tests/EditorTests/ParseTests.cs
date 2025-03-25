using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace JSM.JSME.Tests.EditorTests
{
    public class ParseTests
    {
        public void AssertCanParseLine(string line)
        {
            List<JsmeToken> tokens = JsmeLexing.ToTokens(line, 1).ToList();
            List<List<JsmeToken>> parsed = JsmeParsing.Parse(new []{line}).ToList();
            
            Assert.AreEqual(tokens.Count, parsed[0].Count);
            Assert.AreEqual(tokens, parsed[0]);
        }

        public void AssertParseError(string line)
        {
            try
            {
                List<List<JsmeToken>> parsed = JsmeParsing.Parse(new []{line}).ToList();
                Assert.Fail();
            }
            catch (JsmeParseError)
            {
                Assert.Pass();
            }
        }

        [Test]
        public void CannotParseEmptyTest()
        {
            List<string> emptyStrings = new List<string>() { "", "      " };

            foreach (var empty in emptyStrings)
            {
                AssertParseError(empty);   
            }
        }

        [Test]
        public void CannotParseNonKeywordStart()
        {
            List<string> invalidStrings = new List<string>() { "4 < 5", "\"Boo\"" };
            foreach (var invalid in invalidStrings)
            {
                AssertParseError(invalid);   
            }
        }

        [Test]
        public void CanParseVariableUpdatesTest()
        {
            List<string> validStarts = new List<string>() { "SET", "ADD", "SUB", "MULT", "DIV"};
            List<string> validArgs = new List<string>()
                { "AGE 13", "NAME \"BOO\"", "PERCENTAGE 999.75", "NAME OTHERNAME" };

            foreach (var start in validStarts)
            {
                foreach (var arg in validArgs)
                {
                    AssertCanParseLine($"{start} {arg}");
                }
            }
        }

        [Test]
        public void CannotParseVariableUpdatesOverfilledTest()
        {
            AssertParseError("ADD X 15 WOAW");
        }
        
        [Test]
        public void CannotParseVariableUpdatesIncompleteTest()
        {
            AssertParseError("ADD X");
            AssertParseError("ADD");
        }
        
        
        [Test]
        public void CanParseRunStatements()
        {
            AssertCanParseLine("RUN LoadScene ()");
            AssertCanParseLine("RUN LoadScene()");
            AssertCanParseLine("RUN LoadScene(1)");
            AssertCanParseLine("RUN LoadScene(1 2)");
            AssertCanParseLine("RUN OtherThing(1 \"Hi\" 3)");
            AssertCanParseLine("RUN OtherThing(1 \"Hi\" 3) ONCOMPLETE \"LoadScene\"");
            AssertCanParseLine("RUN OtherThing(1 \"Hi\" 3) ONCOMPLETE \"LoadScene\" IF A = B");
            AssertCanParseLine("RUN OtherThing(1 \"Hi\" 3) IF A = B");
            AssertCanParseLine("RUN Issei.Path()");
            AssertCanParseLine("RUN Issei.Path(1 \"Hi\" 3)");
        }
        
        [Test]
        public void CantParseRunStatements()
        {
            AssertParseError("RUN LoadScene");
            AssertParseError("RUN LoadScene (()");
            AssertParseError("RUN LoadScene ())");
            AssertParseError("RUN \"Hi\"");
            AssertParseError("RUN ADD()");
            AssertParseError("RUN ONCOMPLETE()");
            AssertParseError("RUN OtherThing(1 \"Hi\" 3) ONCOMPLETE \"LoadScene\" IF");
            AssertParseError("RUN OtherThing(1 \"Hi\" 3) ONCOMPLETE IF A = B");
        }

        [Test]
        public void CanParseAsyncStatements()
        {
            AssertCanParseLine("ASYNC RUN LoadScene ()");
        }
        
        
        [Test]
        public void CantParseAsyncStatements()
        {
            AssertParseError("ASYNC LoadScene");
            AssertParseError("ASYNC LoadScene ()");
            AssertParseError("ASYNC ADD ()");
        }
        
        [Test]
        public void CanParseAwaitStatements()
        {
            AssertCanParseLine("AWAIT A = B");
            AssertCanParseLine("AWAIT A >= B");
            AssertCanParseLine("AWAIT A <= 0");
            AssertCanParseLine("AWAIT A <= 0.5");
            AssertCanParseLine("AWAIT A = \"A\"");
        }
                
        [Test]
        public void CantParseAwaitStatements()
        {
            AssertParseError("AWAIT 0.5 = 0.5");
            AssertParseError("AWAIT 0.5 = A");
            AssertParseError("AWAIT RUN A");
            AssertParseError("AWAIT A");
            AssertParseError("AWAIT IF A");
        }
        
        
        [Test]
        public void CanParseLabeledStatements()
        {
            AssertCanParseLine("CoolThing: SET A 5");
            AssertCanParseLine("CoolLine: RUN OtherThing(1 \"Hi\" 3) ONCOMPLETE \"LoadScene\" IF A = B");
        }
        
        [Test]
        public void CantParseLabeledStatements()
        {
            AssertParseError("SET CoolThing: A 5");
            AssertParseError("CoolLine: ");
        }
        
        [Test]
        public void CanParsePrintStatements()
        {
            AssertCanParseLine("PRINT CoolThing");
            AssertCanParseLine("PRINT 5");
            AssertCanParseLine("PRINT 5.0");
            AssertCanParseLine("PRINT \"Hi\"");
        }
        
        [Test]
        public void CantParsePrintStatements()
        {
            AssertParseError("PRINT CoolThing AHHHH");
            AssertParseError("PRINT");
        }
    }
}