using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using JSM.JSME;

public class JsmeRunnerPlayTests
{
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator JsmeRunnerPlayTestsWithEnumeratorPasses()
    {
        JsmeToken MakeDummyToken(JsmeTokenKind kind, object value = null)
        {
            return new JsmeToken(kind, 
                "Dummy", 
                new JsmeLocation(1, 1),
                value);
        }

        string objId = "FHASDFHKsakdfJASDF213";
        
                
        var dummyObject = new GameObject();
        dummyObject.AddComponent<ExampleJSME>();
        dummyObject.AddComponent<JsmeObject>();
        var dummyObjectScript = dummyObject.GetComponent<JsmeObject>();
        dummyObjectScript.SetId(objId);
        
        yield return new WaitForFixedUpdate();
        
        var jsmeRunnerObj = new GameObject();
        jsmeRunnerObj.AddComponent<JsmeRunner>();

        
        JsmeRunner runner = jsmeRunnerObj.GetComponent<JsmeRunner>();
        
        
        List<JsmeToken> tokens = new List<JsmeToken>()
        {
            MakeDummyToken(JsmeTokenKinds.Run, "RUN"),
            MakeDummyToken(JsmeTokenKinds.Identifier, objId),
            MakeDummyToken(JsmeTokenKinds.Dot, "."),
            MakeDummyToken(JsmeTokenKinds.Identifier, "UpdatePosition"),
            MakeDummyToken(JsmeTokenKinds.LeftParenthesis, "("),
            MakeDummyToken(JsmeTokenKinds.LiteralInt, 50),
            MakeDummyToken(JsmeTokenKinds.LiteralInt, -50),
            MakeDummyToken(JsmeTokenKinds.RightParenthesis, ")"),
        };
        
        yield return new WaitForFixedUpdate();
        
        runner.UpdateSceneObjects();
        
        runner.ConvertTokensToCommand(tokens).Execute();
        
        Assert.AreEqual(new Vector2(50,-50), (Vector2)dummyObject.transform.position);
    }
}
