using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace JSM.JSME {
    public class JsmeRunner : MonoBehaviour
    {
        //TODO: This class should handle execution of JSME file given a series of lines

        private JsmeCache _cache;
        private bool _awaiting = false;

        public JsmeCache GetCache()
        {
            return _cache;
        }

        public Func<object>[] GetParamArguments(List<JsmeToken> tokens)
        {
            List<Func<object>> safeArgs = new List<Func<object>>();

            foreach (var token in tokens)
            {
                safeArgs.Add(ReadIdentifier(token));
            }

            return safeArgs.ToArray();
        }

        public Func<object> ReadIdentifier(JsmeToken token)
        {
            return token.Kind() == JsmeTokenKinds.Identifier ?  
                () => _cache.GetVariableDefault(token.Value().ToString()) : 
                token.Value;
        }
        
        public JsmeCommand ConvertTokensToCommand(List<JsmeToken> tokens)
        {
            List<JsmeToken> cleanedTokens = JsmeTokenFiltering.CleanLabel(tokens);

            bool async = false;
            if (cleanedTokens[0].Kind() == JsmeTokenKinds.Async)
            {
                cleanedTokens = cleanedTokens.Skip(1).ToList();
                async = true;
            }
            
            JsmeCommand command = cleanedTokens[0].Kind() switch
            {
                var kind when kind == JsmeTokenKinds.Print => JsmeCommandFactories.MakePrintCommand(cleanedTokens, this),
                var kind when kind == JsmeTokenKinds.Run =>  JsmeCommandFactories.MakeRunCommand(cleanedTokens, this),
                var kind when kind == JsmeTokenKinds.Add => JsmeCommandFactories.MakeAddCommand(cleanedTokens, this),
                var kind when kind == JsmeTokenKinds.Sub => JsmeCommandFactories.MakeSubCommand(cleanedTokens, this),
                var kind when kind == JsmeTokenKinds.Multiply => JsmeCommandFactories.MakeMultiplyCommand(cleanedTokens, this),
                var kind when kind == JsmeTokenKinds.Div => JsmeCommandFactories.MakeDivCommand(cleanedTokens, this),
                var kind when kind == JsmeTokenKinds.Set => JsmeCommandFactories.MakeSetCommand(cleanedTokens, this),
                var kind when kind == JsmeTokenKinds.Await => JsmeCommandFactories.MakeAwaitCommand(cleanedTokens, this),
                _ => throw new ArgumentException("Tokens not convertable to command")
            };

            if (async)
            {
                command = new JsmeAsyncDecorator(command);
            }
            
            return command;
        }

        private Dictionary<string, JsmeObject> GetSceneObjects()
        {
            Dictionary<string, JsmeObject> output = new Dictionary<string, JsmeObject>();

            foreach (var jsmeObject in GameObject.FindObjectsOfType<JsmeObject>())
            {
                output.Add(jsmeObject.GetId(), jsmeObject);
            }

            return output;
        }
        
        //TODO Test this to make sure it works.      
        //This might be slow so check performance methinks
        private Dictionary<string, Delegate> GetGlobalFunctions()
        {
            Component[] components = gameObject.GetComponents<Component>();

            Dictionary<string, Delegate> cachedFunctions = new Dictionary<string, Delegate>();
            
            foreach (var component in components)
            {
                MethodInfo[] methods = component.GetType().GetMethods(BindingFlags.Public|
                                                                      BindingFlags.NonPublic);
                
                foreach (MethodInfo method in methods)
                {
                    string key = $"{component.GetType().Name}.{method.Name}";
                    try
                    {
                        cachedFunctions[key] = Delegate.CreateDelegate(
                            Expression.GetDelegateType(
                                (from parameter in method.GetParameters() select parameter.ParameterType)
                                .Concat(new[] { method.ReturnType })
                                .ToArray()), 
                            component, 
                            method);
                    }
                    catch (ArgumentException ex)
                    {
                        Debug.LogWarning($"Could not create delegate for method: {key}. Error: {ex.Message}");
                    }
                }
            }

            return cachedFunctions;
        }

        public void Awake()
        {
            _cache = new JsmeCache(GetSceneObjects(), GetGlobalFunctions());
        }

        public void UpdateSceneObjects()
        {
            _cache.UpdateObjects(GetSceneObjects());
        }

        private void RunJsmeEvent(IEnumerable<string> lines)
        {
            foreach (var line in JsmeParsing.Parse(lines))
            {
                var command = ConvertTokensToCommand(line);
                var label  =JsmeTokenFiltering.GrabLabel(line);

                if (label != null)
                {
                    _cache.AddCommandToLabelLookup(label.Value().ToString(), command);   
                }
                
                _cache.AddLine(command);
            }

            StartCoroutine(RunScript());
        }

        IEnumerator RunScript()
        {
            foreach (var line in _cache.GetLines())
            {
                line.Execute();
                while (_awaiting)
                {
                    yield return null;
                }
            }
        }
        
        public void StartAwait()
        {
            _awaiting = true;
        }
        
        public void EndAwait()
        {
            _awaiting = false;
        }
        
        public void Start()
        {
             RunJsmeEvent(new[]
             {
                 "Thing: RUN Issei.PrintThing(\"WAOW\") IF MyNumber <= 500",
                 "AWAIT MyNumber = 500",
                 "RUN Issei.PrintThing(\"Yooo\")",
                 "RUN Issei.PrintThing(MyText) ONCOMPLETE \"Thing\"",
                 "UpdateNumber: SET MyNumber 500",
             });
        }
        
    }
}