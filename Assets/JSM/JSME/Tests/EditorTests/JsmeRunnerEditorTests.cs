using System;
using System.Collections.Generic;
using NUnit.Framework;
using JSM.JSME;
using UnityEngine;

namespace JSM.JSME.Tests.EditorTests
{
    public class JsmeRunnerEditorTests
    {
        JsmeToken MakeDummyToken(JsmeTokenKind kind, object value = null)
        {
            return new JsmeToken(kind, 
                "Dummy", 
                new JsmeLocation(1, 1),
                value);
        }
        
        [Test]
        public void GetParamTokensBasicTest()
        {
            List<JsmeToken> tokens = new List<JsmeToken>()
            {
                MakeDummyToken(JsmeTokenKinds.Run, "RUN"),
                MakeDummyToken(JsmeTokenKinds.Identifier, "Thing"),
                MakeDummyToken(JsmeTokenKinds.LeftParenthesis, "("),
                MakeDummyToken(JsmeTokenKinds.Identifier, "Thing"),
                MakeDummyToken(JsmeTokenKinds.RightParenthesis, ")"),
            };
            
            
            List<JsmeToken> paramTokens = JsmeTokenFiltering.GetParamTokens(tokens);
            
            Assert.AreEqual(new [] {MakeDummyToken(JsmeTokenKinds.Identifier, "Thing"),}, paramTokens);
        }
        
        [Test]
        public void GetParamTokensEmptyTest()
        {
            List<JsmeToken> tokens = new List<JsmeToken>()
            {
                MakeDummyToken(JsmeTokenKinds.Run, "RUN"),
                MakeDummyToken(JsmeTokenKinds.Identifier, "Thing"),
                MakeDummyToken(JsmeTokenKinds.LeftParenthesis, "("),
                MakeDummyToken(JsmeTokenKinds.RightParenthesis, ")"),
            };
            
            List<JsmeToken> paramTokens = JsmeTokenFiltering.GetParamTokens(tokens);
            
            Assert.AreEqual(new List<JsmeToken>(), paramTokens);
        }
        
        [Test]
        public void GetParamTokensInvalidTestRight()
        {
            List<JsmeToken> tokens = new List<JsmeToken>()
            {
                MakeDummyToken(JsmeTokenKinds.Run, "RUN"),
                MakeDummyToken(JsmeTokenKinds.Identifier, "Thing"),
                MakeDummyToken(JsmeTokenKinds.RightParenthesis, ")"),
            };

            try
            {
                List<JsmeToken> paramTokens = JsmeTokenFiltering.GetParamTokens(tokens);
                Assert.Fail();
            }
            catch (ArgumentException)
            {
                Assert.Pass();
            }
        }
        
        [Test]
        public void GetParamTokensInvalidTestLeft()
        {
            List<JsmeToken> tokens = new List<JsmeToken>()
            {
                MakeDummyToken(JsmeTokenKinds.Run, "RUN"),
                MakeDummyToken(JsmeTokenKinds.Identifier, "Thing"),
                MakeDummyToken(JsmeTokenKinds.RightParenthesis, "("),
            };

            try
            {
                List<JsmeToken> paramTokens = JsmeTokenFiltering.GetParamTokens(tokens);
                Assert.Fail();
            }
            catch (ArgumentException)
            {
                Assert.Pass();
            }
        }

        [Test]
        public void CleanLabelTest()
        {
            List<JsmeToken> tokensWithLabel = new List<JsmeToken>()
            {
                MakeDummyToken(JsmeTokenKinds.Identifier, "Thing"),
                MakeDummyToken(JsmeTokenKinds.Colon, ":"),
                MakeDummyToken(JsmeTokenKinds.Run, "RUN"),
                MakeDummyToken(JsmeTokenKinds.Identifier, "Args"),
            };

            List<JsmeToken> cleanedTokens = JsmeTokenFiltering.CleanLabel(tokensWithLabel);

            Assert.AreEqual(new [] {MakeDummyToken(JsmeTokenKinds.Run, "RUN"), 
                MakeDummyToken(JsmeTokenKinds.Identifier, "Args")}, 
                cleanedTokens);

        }
        
        [Test]
        public void CleanLabelUnlabeledTest()
        {
            List<JsmeToken> tokensWithLabel = new List<JsmeToken>()
            {
                MakeDummyToken(JsmeTokenKinds.Run, "RUN"),
                MakeDummyToken(JsmeTokenKinds.Identifier, "Args"),
            };

            List<JsmeToken> cleanedTokens = JsmeTokenFiltering.CleanLabel(tokensWithLabel);

            Assert.AreEqual(new [] {MakeDummyToken(JsmeTokenKinds.Run, "RUN"), 
                    MakeDummyToken(JsmeTokenKinds.Identifier, "Args")}, 
                cleanedTokens);

        }
        
        [Test]
        public void GetOnCompleteTokensTest()
        {
            List<JsmeToken> fullStatement = new List<JsmeToken>()
            {
                MakeDummyToken(JsmeTokenKinds.Run, "RUN"),
                MakeDummyToken(JsmeTokenKinds.Identifier, "Thing"),
                MakeDummyToken(JsmeTokenKinds.LeftParenthesis, "("),
                MakeDummyToken(JsmeTokenKinds.RightParenthesis, ")"),
                MakeDummyToken(JsmeTokenKinds.OnComplete, "ONCOMPLETE"),
                MakeDummyToken(JsmeTokenKinds.Identifier, "Func"),
            };

            List<JsmeToken> onCompleteTokens = JsmeTokenFiltering.GetOnCompleteTokens(fullStatement);

            Assert.AreEqual(new [] {MakeDummyToken(JsmeTokenKinds.Identifier, "Func")}, 
                onCompleteTokens);
        }
        
        [Test]
        public void GetOnCompleteTokensNoCompleteTest()
        {
            List<JsmeToken> fullStatement = new List<JsmeToken>()
            {
                MakeDummyToken(JsmeTokenKinds.Run, "RUN"),
                MakeDummyToken(JsmeTokenKinds.Identifier, "Thing"),
                MakeDummyToken(JsmeTokenKinds.LeftParenthesis, "("),
                MakeDummyToken(JsmeTokenKinds.RightParenthesis, ")"),
            };

            List<JsmeToken> onCompleteTokens = JsmeTokenFiltering.GetOnCompleteTokens(fullStatement);

            Assert.AreEqual(onCompleteTokens.Count, 
                0);
        }
        
        [Test]
        public void GetIfTokensTest()
        {
            List<JsmeToken> fullStatement = new List<JsmeToken>()
            {
                MakeDummyToken(JsmeTokenKinds.Run, "RUN"),
                MakeDummyToken(JsmeTokenKinds.Identifier, "Thing"),
                MakeDummyToken(JsmeTokenKinds.LeftParenthesis, "("),
                MakeDummyToken(JsmeTokenKinds.RightParenthesis, ")"),
                MakeDummyToken(JsmeTokenKinds.If, "IF"),
                MakeDummyToken(JsmeTokenKinds.Identifier, "Func"),
                MakeDummyToken(JsmeTokenKinds.Equal, "="),
                MakeDummyToken(JsmeTokenKinds.LiteralStr, "Ahhha"),
            };

            List<JsmeToken> ifTokens = JsmeTokenFiltering.GetIfTokens(fullStatement);

            Assert.AreEqual(new []
                {
                    MakeDummyToken(JsmeTokenKinds.Identifier, "Func"),
                    MakeDummyToken(JsmeTokenKinds.Equal, "="),
                    MakeDummyToken(JsmeTokenKinds.LiteralStr, "Ahhha"),
                }, 
                ifTokens);
        }
        
        [Test]
        public void GetIfNoIfClauseTest()
        {
            List<JsmeToken> fullStatement = new List<JsmeToken>()
            {
                MakeDummyToken(JsmeTokenKinds.Run, "RUN"),
                MakeDummyToken(JsmeTokenKinds.Identifier, "Thing"),
                MakeDummyToken(JsmeTokenKinds.LeftParenthesis, "("),
                MakeDummyToken(JsmeTokenKinds.RightParenthesis, ")"),
            };

            List<JsmeToken> ifTokens = JsmeTokenFiltering.GetIfTokens(fullStatement);

            Assert.AreEqual(ifTokens.Count, 
                0);
        }
        
    }
}