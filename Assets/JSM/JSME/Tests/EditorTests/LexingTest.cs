using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using JSM.JSME;
using System.Linq;
using System.Xml.Schema;

public class LexingTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void LexingTestSimplePasses()
    {
        List<JsmeTokenKind> expected = new List<JsmeTokenKind>();
        
        expected.Add(JsmeTokenKinds.Identifier);
        expected.Add(JsmeTokenKinds.Colon);
        expected.Add(JsmeTokenKinds.Identifier);
        expected.Add(JsmeTokenKinds.Goto);
        expected.Add(JsmeTokenKinds.LiteralInt);
        
        List<JsmeTokenKind> output = JsmeLexing.ToTokens("Label: A GOTO 5", 1).Select(x=>x.Kind()).ToList();

        Assert.AreEqual(output, expected);
    }

    private void AssertNoToken(string line)
    {
        List<JsmeToken> output = JsmeLexing.ToTokens(line, 1).ToList();
        
        Assert.AreEqual(0, output.Count);
    }
    
    private void AssertOneToken(string line, JsmeTokenKind kind, string text, object value=null)
    {
        List<JsmeToken> output = JsmeLexing.ToTokens(line, 1).ToList();
        
        Assert.AreEqual(1, output.Count);
        Assert.AreEqual(kind, output[0].Kind());
        Assert.AreEqual(text, output[0].Text());
        Assert.AreEqual(value, output[0].Value());
    }
    
    [Test]
    public void EmptyStringTest()
    {
        AssertNoToken("");
    }
    
    [Test]
    public void WhiteSpaceStringTest()
    {
        AssertNoToken("          ");
    }

    [Test]
    public void CanRecognizeKeywordsTest()
    {
        foreach (var key in JsmeLexing.TokenKeys)
        {
            List <JsmeToken> tokens = JsmeLexing.ToTokens(key, 1).ToList();
            
            Assert.AreEqual(1, tokens.Count);
            Assert.AreNotEqual(JsmeTokenKinds.Identifier, tokens[0].Kind());
            Assert.AreEqual(key, tokens[0].Text());
            Assert.AreEqual(key, tokens[0].Value());
        }
    }

    [Test]
    public void CanRecognizeIdentifiersTest()
    {
        List<string> testText = new List<string>() {"Boo", "U2", "THIS1ISTHELAST1"};

        foreach (string text in testText)
        {
            AssertOneToken(text, JsmeTokenKinds.Identifier, text, text);
        }
    }
    
    [Test]
    public void CanRecognizeStringLiteralsTest()
    {
        List<string> testText = new List<string>() {"\"Boo\"", "\"Hello Boo!\""};

        foreach (string text in testText)
        {
            AssertOneToken(text, JsmeTokenKinds.LiteralStr, text, text.Substring(1, text.Length-2));
        }
    }

    [Test]
    public void CanRecognizeIntLiteralsTest()
    {
        List<(string,int)> testText = new List<(string,int)>() {("123",123), ("-1", -1),("0",0)};

        foreach ((string,int) text in testText)
        {
            AssertOneToken(text.Item1, JsmeTokenKinds.LiteralInt, text.Item1, text.Item2);
        }
    }
    
    [Test]
    public void CanRecognizeFloatLiteralsTest()
    {
        List<(string,float)> testText = new List<(string,float)>() {("123.5",123.5f), ("-1.5", -1.5f),("0.2",0.2f)};

        foreach ((string,float) text in testText)
        {
            AssertOneToken(text.Item1, JsmeTokenKinds.LiteralFloat, text.Item1, text.Item2);
        }
    }
    
    [Test]
    public void CanRecognizeNegationWithoutNumberTest()
    {
        try
        {
            IEnumerable<JsmeToken> things = JsmeLexing.ToTokens("-", 1).ToList();
            Assert.Fail();
        }
        catch (JsmeLexError)
        {
            Assert.Pass();
        }
    }

    [Test]
    public void UnterminatedStingLiteralTest()
    {
        try
        {
            IEnumerable<JsmeToken> things = JsmeLexing.ToTokens("\"This does not end", 1).ToList();
            Assert.Fail();
        }
        catch (JsmeLexError)
        {
            Assert.Pass();
        }
    }
    
    [Test]
    public void RecognizeComparisonOperationsTest()
    {
        AssertOneToken("=", JsmeTokenKinds.Equal, "=");
        AssertOneToken("!=", JsmeTokenKinds.NotEqual, "!=");
        AssertOneToken("<", JsmeTokenKinds.LessThan, "<");
        AssertOneToken("<=", JsmeTokenKinds.LessThanOrEqual, "<=");
        AssertOneToken(">", JsmeTokenKinds.GreaterThan, ">");
        AssertOneToken(">=", JsmeTokenKinds.GreaterThanOrEqual, ">=");
    }
}
