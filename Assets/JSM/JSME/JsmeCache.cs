using System;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace JSM.JSME
{
    public class JsmeCache
    {
        private readonly Dictionary<string, JsmeCommand> _commandLookup = new Dictionary<string, JsmeCommand>();
        private readonly Dictionary<string, Delegate> _globalFunctions;
        private readonly List<JsmeCommand> _lines = new List<JsmeCommand>();
        private Dictionary<string, JsmeObject> _jsmeObjects;

        public JsmeCache(Dictionary<string, JsmeObject> jsmeObjects, Dictionary<string, Delegate>  globalFunctions = null)
        {
            _jsmeObjects = jsmeObjects;
            _globalFunctions = globalFunctions;
        }

        public void AddCommandToLabelLookup(string key, JsmeCommand value)
        {
            _commandLookup.TryAdd(key, value);
        }

        public JsmeCommand GetLookup(string key)
        {
            var command = _commandLookup.GetValueOrDefault(key, null);
            if (command == null)
            {
                throw new ArgumentException($"Invalid Command lookup key {key}");
            }
            return command;
        }
        
        public void UpdateObjects(Dictionary<string, JsmeObject> jsmeObjects)
        {
            _jsmeObjects = jsmeObjects;
        }

        public void AddLine(JsmeCommand line)
        {
            _lines.Add(line);
        }
        
        public List<JsmeCommand> GetLines()
        {
            return _lines;
        }
        
        public T GetVariable<T>(string key)
        {
            if (!DialogueLua.DoesVariableExist(key))
            {
                throw new ArgumentException("Variable not found");
            }   
            
            return typeof(T) switch
            {
                var x when x == typeof(int) => (T)Convert.ChangeType(DialogueLua.GetVariable(key). AsInt, typeof(T)),
                var x when x == typeof(float) => (T)Convert.ChangeType(DialogueLua.GetVariable(key).AsFloat, typeof(T)),
                var x when x == typeof(string) => (T)Convert.ChangeType(DialogueLua.GetVariable(key).AsString, typeof(T)),
                var x when x == typeof(bool) => (T)Convert.ChangeType(DialogueLua.GetVariable(key).AsBool, typeof(T)),
                
                _ => throw new ArgumentException("Invalid Type")
            };
        }

        public void UpdateVariable<T>(string key, T value)
        {
            if (!DialogueLua.DoesVariableExist(key))
            {
                throw new ArgumentException("Variable not found");
            }   
            
            DialogueLua.SetVariable(key, value);
        }
        
        public object GetVariableDefault(string key)
        {
            if (!DialogueLua.DoesVariableExist(key))
            {
                throw new ArgumentException("Variable not found");
            }   
            
            Lua.Result result = DialogueLua.GetVariable(key);
            if (result.isNumber)
            {
                float val = result.AsFloat;
                bool isInt = int.TryParse(val.ToString(), out var x);

                if (isInt)
                {
                    return x;
                }

                return val;
            }
            else if (result.isBool) {
                return result.AsBool;
            }
            else {
                return result.AsString;
            }
        }

        public Delegate GetGlobalDelegate(string key)
        {
            if (!_globalFunctions.TryGetValue(key, out var func))
            {
                throw new ArgumentException("Command does not exist");
            }

            return func;
        }
        
        public Delegate GetObjectDelegate(string key, string functionName)
        {
            if (!_jsmeObjects.TryGetValue(key, out var jsmeObject))
            {
                throw new ArgumentException("Object does not exist");
            }

            return jsmeObject.GetDelegate(functionName);
        }
    }
}