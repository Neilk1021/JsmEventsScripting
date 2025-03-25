using System;
using System.Collections.Generic;
using System.Linq;

namespace JSM.JSME
{
    public static class JsmeTokenFiltering
    {
        public static List<JsmeToken> CleanLabel(List<JsmeToken> tokens)
        {
            if (tokens[0].Kind() == JsmeTokenKinds.Identifier)
            {
                return tokens.Skip(2).ToList(); 
            }

            return tokens;
        }
        
        public static JsmeToken GrabLabel(List<JsmeToken> tokens)
        {
            if (tokens[0].Kind() == JsmeTokenKinds.Identifier)
            {
                return tokens[0];
            }

            return null;
        }


        //TODO TEST
        public static List<JsmeToken> GetParamTokens(List<JsmeToken> tokens)
        {
            int startIndex = -1;
            int stopIndex = -1;
            for (int i = 0; i < tokens.Count; i++) {
                if (tokens[i].Kind() == JsmeTokenKinds.LeftParenthesis) {
                    startIndex = i+1;
                }
                
                if (tokens[i].Kind() == JsmeTokenKinds.RightParenthesis) {
                    stopIndex = i;
                    break;
                }
            }

            if (startIndex == -1) {
                throw new ArgumentException("'(' missing from tokens");
            }

            if (stopIndex == -1) {
                throw new ArgumentException("')' missing from tokens");
            }
            
            return tokens.Skip(startIndex).Take(stopIndex - startIndex).ToList();
        }
        
        public static List<JsmeToken> GetOnCompleteTokens(List<JsmeToken> tokens)
        {
            for (int i = 0; i < tokens.Count; i++) {
                if (tokens[i].Kind() == JsmeTokenKinds.OnComplete)
                {
                    return new List<JsmeToken>() { tokens[i + 1] };
                }
            }

            return new List<JsmeToken>();
        }
        
        public static List<JsmeToken> GetIfTokens(List<JsmeToken> tokens)
        {
            for (int i = 0; i < tokens.Count; i++) {
                if (tokens[i].Kind() == JsmeTokenKinds.If)
                {
                    // should always be 3 tokens that we're taking
                    return tokens.Skip(i + 1).Take(3).ToList();
                }
            }

            return new List<JsmeToken>();
        }
        
        public static List<JsmeToken> GetAwaitTokens(List<JsmeToken> tokens)
        {
            for (int i = 0; i < tokens.Count; i++) {
                if (tokens[i].Kind() == JsmeTokenKinds.Await)
                {
                    // should always be 3 tokens that we're taking
                    return tokens.Skip(i + 1).Take(3).ToList();
                }
            }

            return new List<JsmeToken>();
        }

        
        public static List<JsmeToken> GetAsync(List<JsmeToken> tokens)
        {
            for (int i = 0; i < tokens.Count; i++) {
                if (tokens[i].Kind() == JsmeTokenKinds.If)
                {
                    // should always be 3 tokens that we're taking
                    return tokens.Skip(i + 1).Take(3).ToList();
                }
            }

            return new List<JsmeToken>();
        }
    }
}