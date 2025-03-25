using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JSM.JSME
{
    public class JsmeLexError : Exception
    {
        public JsmeLexError()
        {
        }
        
        public JsmeLexError(string message):base(message)
        {
            
        }
    }
    
    public static class JsmeLexing
    {
        //Dict of all possible keywords
        public static readonly Dictionary<string, JsmeTokenKind> TokenKindMap = 
            JsmeTokenKinds.TokenKinds
                .Where(val => val.Category == JsmeTokenCategory.Keyword)
                .Select(item => new { Key = item.Name, Value = item })
                .ToDictionary(x => x.Key, x => x.Value);

        public static List<string> TokenKeys = TokenKindMap.Select(x => x.Key).ToList();
        
        public static IEnumerable<JsmeToken> ToTokens(string line, int lineNumber)
        {
            int index = 0;
            int start = 0;

            JsmeToken MakeToken(JsmeTokenKind kind, object value = null)
            {
                return new JsmeToken(kind, 
                    line.Substring(start, index-start), 
                    new JsmeLocation(lineNumber, start + 1),
                    value);
            }

            void RaiseError(string message)
            {
                throw new JsmeLexError(message);
            }

            while (true)
            {
                index = SkipWhiteSpace(line, index);

                if (index == line.Length)
                    break;


                start = index;
                if (char.IsLetter(line[index]))
                {
                    index = FindEndOfIdentifier(line, index);

                    string identifier = line.Substring(start, index-start);
                    
                    JsmeTokenKind keyType = 
                        TokenKindMap.TryGetValue(identifier, out var tokenKind) ? tokenKind : JsmeTokenKinds.Identifier;

                    yield return MakeToken(keyType, identifier);
                }
                else if(line[index] == '"')
                {
                    index = FindEndOfStringLiteral(line, index);

                    if (index >= line.Length) {
                        RaiseError("Newline in literal");
                    }
                    else {
                        index += 1;
                        string substring = line.Substring(start+1, index-2-start);   
                        yield return MakeToken(JsmeTokenKinds.LiteralStr, substring);
                    }
                }
                else if (char.IsNumber(line[index]) || line[index] == '-')
                {
                    bool isNegated = line[index] == '-';
                    index += 1;
                    //We have 1 digit if no negative sign, otherwise we only have 0
                    int digits = isNegated ? 0 : 1;

                    while (index < line.Length && char.IsNumber(line[index])) {
                        digits += 1;
                        index += 1;
                    }

                    if (digits == 0) {
                        RaiseError("Negation '-' must be followed by digit");
                    }
                    
                    else if (index < line.Length && line[index] == '.') {
                        index += 1;

                        while (index < line.Length && char.IsNumber(line[index])) {
                            index += 1;
                        }
                        string substring = line.Substring(start, index-start);
                        yield return MakeToken(JsmeTokenKinds.LiteralFloat, float.Parse(substring));
                    }
                    else {
                        string substring = line.Substring(start, index-start);
                        yield return MakeToken(JsmeTokenKinds.LiteralInt, int.Parse(substring));
                    }
                }
                else{
                    switch (line[index])
                    {
                        case ':':
                            index += 1;
                            yield return MakeToken(JsmeTokenKinds.Colon);
                            break;
                        case '(':
                            index += 1;
                            yield return MakeToken(JsmeTokenKinds.LeftParenthesis);
                            break;
                        case ')':
                            index += 1;
                            yield return MakeToken(JsmeTokenKinds.RightParenthesis);
                            break;
                        case '.':
                            index += 1;
                            yield return MakeToken(JsmeTokenKinds.Dot);
                            break;
                        case '=':
                            index += 1;
                            yield return MakeToken(JsmeTokenKinds.Equal);
                            break;
                        case '!':
                            index += 1;
                            if (line.Length > index && line[index] == '=')
                            {
                                index += 1;
                                yield return MakeToken(JsmeTokenKinds.NotEqual);
                            }
                            else
                            {
                                RaiseError("Not symbol without matching Equal symbol");
                            }
                            break;
                        case '<':
                            index += 1;
                            if (line.Length > index && line[index] == '=')
                            {
                                index += 1;
                                yield return MakeToken(JsmeTokenKinds.LessThanOrEqual);
                            }
                            else
                            {
                                yield return MakeToken(JsmeTokenKinds.LessThan);
                            }
                            break;
                        case '>':
                            index += 1;
                            if (line.Length > index &&  line[index] == '=')
                            {
                                index += 1;
                                yield return MakeToken(JsmeTokenKinds.GreaterThanOrEqual);
                            }
                            else
                            {
                                yield return MakeToken(JsmeTokenKinds.GreaterThan);
                            }
                            break;
                        default:
                            RaiseError("Unsupported character");
                            break;
                    }
                }
            }

        }

        private static int FindEndOfIdentifier(string line, int index)
        {
            index += 1;
            while (index < line.Length && char.IsLetterOrDigit(line[index])) {
                index += 1;
            }
            return index;
        }
        
        private static int FindEndOfStringLiteral(string line, int index)
        {
            index += 1;
            while (index < line.Length && line[index] != '"') {
                index += 1;
            }
            return index;
        }


        private static int SkipWhiteSpace(string line, int index)
        {
            while (index < line.Length && line[index] == ' ') {
                index += 1;
            }
            return index;
        }
    }
}