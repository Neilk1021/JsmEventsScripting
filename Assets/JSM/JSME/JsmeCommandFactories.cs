using System;
using System.Collections.Generic;
using UnityEngine;

namespace JSM.JSME
{
    public static class JsmeCommandFactories
    {
        public static JsmePrintCommand MakePrintCommand(List<JsmeToken> tokens, JsmeRunner runner)
        {
            if (tokens[1].Kind() == JsmeTokenKinds.Identifier)
            {
                return new JsmePrintCommand(runner, runner.GetCache().GetVariable<string>(tokens[1].Value().ToString()));   
            }
            
            return new JsmePrintCommand(runner, tokens[1].Value().ToString());   
        }
        
        private static JsmeRunCommand MakeBasicRunCommand(List<JsmeToken> tokens, JsmeRunner runner)
        {
            Func<object>[] args  = runner.GetParamArguments(JsmeTokenFiltering.GetParamTokens(tokens));

            if (tokens[2].Kind() == JsmeTokenKinds.Dot)
            {
                Delegate f =  runner.GetCache().GetObjectDelegate(tokens[1].Value().ToString(), tokens[3].Value().ToString());
                return new JsmeRunCommand(runner, f, args);
            }
            else
            {
                //TODO FILL OUT GLOBAL METHOD Calls
                Delegate f = runner.GetCache().GetGlobalDelegate(tokens[1].Value().ToString());
                return new JsmeRunCommand(runner, f, args);
            }
        }
        
        private static string GetCommandLabel(JsmeToken token, JsmeRunner runner)
        {
            return runner.ReadIdentifier(token)().ToString();
        }
        
        private static Func<bool> MakeConditional(List<JsmeToken> ifTokens, JsmeRunner runner)
        {
            List<Func<object>> safeArgs = new List<Func<object>>();

            foreach (var token in ifTokens)
            {
                safeArgs.Add(runner.ReadIdentifier(token));
            }

            switch (ifTokens[1].Kind())
            {
                case var kind when kind == JsmeTokenKinds.LessThan:
                    return () => float.Parse(safeArgs[0]().ToString()) < float.Parse(safeArgs[2]().ToString());
                case var kind when kind == JsmeTokenKinds.LessThanOrEqual:
                    return () =>  float.Parse(safeArgs[0]().ToString()) <= float.Parse(safeArgs[2]().ToString());
                case var kind when kind == JsmeTokenKinds.GreaterThan:
                    return () => float.Parse(safeArgs[0]().ToString()) > float.Parse(safeArgs[2]().ToString());
                case var kind when kind == JsmeTokenKinds.GreaterThanOrEqual:
                    return () => float.Parse(safeArgs[0]().ToString()) >= float.Parse(safeArgs[2]().ToString());
                case var kind when kind == JsmeTokenKinds.Equal:
                    return () => JsmeCompare.Equal(safeArgs[0](), safeArgs[2]());
                case var kind when kind == JsmeTokenKinds.NotEqual:
                    return () => !JsmeCompare.Equal(safeArgs[0](), safeArgs[2]());
                default:
                    throw new ArgumentException("Invalid Comparison operator used");
            }
        }
        
        public static JsmeCommand MakeRunCommand(List<JsmeToken> tokens, JsmeRunner runner)
        {
            JsmeRunCommand basicRunCommand = MakeBasicRunCommand(tokens, runner);

            JsmeCommandDecorator decoratedCommand = new JsmeCommandDecorator(basicRunCommand);
            
            List<JsmeToken> onCompleteTokens = JsmeTokenFiltering.GetOnCompleteTokens(tokens);
            
            if (onCompleteTokens.Count != 0)
            {
                decoratedCommand = new JsmeOnCompleteDecorator(basicRunCommand, GetCommandLabel(onCompleteTokens[0], runner));
            }
            
            List<JsmeToken> ifTokens = JsmeTokenFiltering.GetIfTokens(tokens);
            if (ifTokens.Count != 0)
            {
                decoratedCommand = new JsmeConditionalDecorator(basicRunCommand, MakeConditional(ifTokens, runner));
            }
            
            return decoratedCommand;
        }

        //TODO ADD If checks
        public static JsmeCommand MakeAddCommand(List<JsmeToken> cleanedTokens, JsmeRunner jsmeRunner)
        {
            float.TryParse(cleanedTokens[2].Value().ToString(), out var x);
            return new JsmeAddCommand(jsmeRunner, cleanedTokens[1].Value().ToString(), x);
        }
        
        //TODO ADD If checks
        public static JsmeCommand MakeSubCommand(List<JsmeToken> cleanedTokens, JsmeRunner jsmeRunner)
        {
            float.TryParse(cleanedTokens[2].Value().ToString(), out var x);
            return new JsmeSubCommand(jsmeRunner, cleanedTokens[1].Value().ToString(), x);
        }
        
        //TODO ADD If checks
        public static JsmeCommand MakeMultiplyCommand(List<JsmeToken> cleanedTokens, JsmeRunner jsmeRunner)
        {
            float.TryParse(cleanedTokens[2].Value().ToString(), out var x);
            return new JsmeMultiplyCommand(jsmeRunner, cleanedTokens[1].Value().ToString(), x);
        }
        
        //TODO ADD If checks
        public static JsmeCommand MakeDivCommand(List<JsmeToken> cleanedTokens, JsmeRunner jsmeRunner)
        {
            float.TryParse(cleanedTokens[2].Value().ToString(), out var x);
            return new JsmeDivideCommand(jsmeRunner, cleanedTokens[1].Value().ToString(), x);
        }
        
        //TODO ADD If checks
        public static JsmeCommand MakeSetCommand(List<JsmeToken> cleanedTokens, JsmeRunner jsmeRunner)
        {
            return new JsmeSetCommand(jsmeRunner, cleanedTokens[1].Value().ToString(), cleanedTokens[2].Value());
        }

        //Make it work with functions
        public static JsmeCommand MakeAwaitCommand(List<JsmeToken> cleanedTokens, JsmeRunner jsmeRunner)
        {
            List<JsmeToken> awaitTokens = JsmeTokenFiltering.GetAwaitTokens(cleanedTokens);
            return new JsmeAwaitCommand(jsmeRunner, MakeConditional(awaitTokens, jsmeRunner));
        }
    }
}