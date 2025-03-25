using System;
using System.Collections.Generic;
using System.Linq;

namespace JSM.JSME
{
    public class JsmeParseError : Exception
    {
        public JsmeParseError()
        {
        }
        
        public JsmeParseError(string message):base(message)
        {
            
        }
    }

    public static class JsmeParsing
    {
        public static IEnumerable<List<JsmeToken>> Parse(IEnumerable<string> lines)
        {
            int lineNumber = 1;
            foreach (var line in lines)
            {
                yield return ParseLine(line, lineNumber);
                lineNumber += 1;
            }
        }
        
        
        private static List<JsmeToken> ParseLine(string line, int lineNumber)
        {
            List<JsmeToken> tokens = JsmeLexing.ToTokens(line, lineNumber).ToList();
            int index = 0;
            
            
            void ThrowErrorOnToken(string message, JsmeToken token) {
                throw new JsmeParseError($"Error during parsing: {token.Location().Line} {token.Location().Column}: {message}");
            }

            void ThrowErrorAtEndOfLine(string message) {
                JsmeLocation loc = new JsmeLocation(lineNumber, index + 1);
                throw new JsmeParseError($"Error during parsing: {loc.Line} {loc.Column}: {message}");
            }
            
            bool TokenIs(IEnumerable<JsmeTokenKind> tokenKinds) {
                return tokens.Count > index && tokenKinds.Contains(tokens[index].Kind());
            }

            void ExpectTokens(IEnumerable<JsmeTokenKind> tokenKinds)
            {
                var jsmeTokenKinds = tokenKinds as JsmeTokenKind[] ?? tokenKinds.ToArray();
                
                if (!TokenIs(jsmeTokenKinds))
                {
                    string message = string.Join(", ", jsmeTokenKinds.Select(x => x.Name));
                    if (index >= tokens.Count)
                    {
                        ThrowErrorAtEndOfLine(message);           
                    }
                    else
                    {
                        ThrowErrorOnToken(message, tokens[index]);  
                    }
                }
            }

            void ParseLabel()
            {
                if (TokenIs(new [] { JsmeTokenKinds.Identifier }))
                {
                    index += 1;
                    ExpectTokens(new [] { JsmeTokenKinds.Colon });
                    index += 1;
                }
            }

            void ParseVariableUpdate()
            {
                ExpectTokens(new [] { JsmeTokenKinds.Identifier });
                index += 1;
                ParseValue();
            }

            void ParseValue()
            {
                ExpectTokens(new []
                    { 
                        JsmeTokenKinds.LiteralFloat, 
                        JsmeTokenKinds.LiteralInt, 
                        JsmeTokenKinds.LiteralStr, 
                        JsmeTokenKinds.Identifier
                    });

                index += 1;
            }

            void ParseJump()
            {
                ParseJumpTarget();

                if (TokenIs(new [] { JsmeTokenKinds.If }))
                {
                    ParseIfBody();
                }
            }
            
            void ParseIfBody()
            {
                index += 1;
                ParseValue();
                ParseComparison(); 
                ParseValue();
            }
            
            void ParseJumpTarget()
            {
                ExpectTokens(new []
                { 
                    JsmeTokenKinds.LiteralInt, 
                    JsmeTokenKinds.LiteralStr, 
                    JsmeTokenKinds.Identifier
                });
                index += 1;
            }

            void ParseComparison()
            {
                ExpectTokens(new []
                { 
                    JsmeTokenKinds.Equal, 
                    JsmeTokenKinds.NotEqual, 
                    JsmeTokenKinds.LessThan, 
                    JsmeTokenKinds.LessThanOrEqual,
                    JsmeTokenKinds.GreaterThan,
                    JsmeTokenKinds.GreaterThanOrEqual
                });

                index += 1;
            }

            void ParseEmpty()
            {
                return;
            }

            void ParseAwait()
            {
                ExpectTokens(new [] {JsmeTokenKinds.Identifier});
                index += 1;
                ParseComparison();
                ParseValue();
            }

            void ParseAsync()
            {
                if (TokenIs(new [] { JsmeTokenKinds.Async }))
                {
                    index += 1;
                    ExpectTokens(new [] { JsmeTokenKinds.Run });
                }
            }

            void ParseRun()
            {
                ExpectTokens(new [] {JsmeTokenKinds.Identifier});
                index += 1;

                if (TokenIs(new [] { JsmeTokenKinds.Dot }))
                {
                    index += 1;
                    ExpectTokens(new []{JsmeTokenKinds.Identifier});
                    index += 1;
                }
                
                ExpectTokens(new []{JsmeTokenKinds.LeftParenthesis});
                index += 1;

                List<JsmeTokenKind> exitState = new List<JsmeTokenKind>() { JsmeTokenKinds.RightParenthesis };
                List<JsmeTokenKind> expectedValues = new List<JsmeTokenKind>()
                {
                    JsmeTokenKinds.LiteralFloat,
                    JsmeTokenKinds.LiteralInt,
                    JsmeTokenKinds.LiteralStr,
                    JsmeTokenKinds.Identifier
                };
                while (true)
                {
                    if (TokenIs(exitState))
                    {
                        index += 1;
                        break;
                    }
                    
                    ExpectTokens(expectedValues);
                    index += 1;
                }

                if (TokenIs(new [] { JsmeTokenKinds.OnComplete }))
                {
                    index += 1;
                    ExpectTokens(new [] { JsmeTokenKinds.LiteralStr, JsmeTokenKinds.Identifier});
                    index += 1;
                } 
                
                if (TokenIs(new [] { JsmeTokenKinds.If }))
                {
                    ParseIfBody();
                } 
            }
            
            Dictionary<JsmeTokenKind, Action> BodyParsers = new Dictionary<JsmeTokenKind, Action>()
            {
                {JsmeTokenKinds.Set, ParseVariableUpdate},
                {JsmeTokenKinds.Add, ParseVariableUpdate},
                {JsmeTokenKinds.Sub, ParseVariableUpdate},
                {JsmeTokenKinds.Multiply, ParseVariableUpdate},
                {JsmeTokenKinds.Div, ParseVariableUpdate},
                {JsmeTokenKinds.Goto, ParseJump},
                {JsmeTokenKinds.End, ParseEmpty},
                {JsmeTokenKinds.Await, ParseAwait},
                {JsmeTokenKinds.Run, ParseRun},
                {JsmeTokenKinds.Print, ParsePrint},
            };

            void ParsePrint()
            {
                ParseValue();
            }

            void ParseBody()
            {
                if (BodyParsers.ContainsKey(tokens[index].Kind()))
                {
                    JsmeTokenKind tokenKind = tokens[index].Kind();
                    index += 1;
                    BodyParsers[tokenKind].Invoke();
                }
                else
                {
                    ThrowErrorOnToken("Expected Statement Token", tokens[index]);
                }
            }

            if (tokens.Count == 0)
            {
                ThrowErrorAtEndOfLine("Program lines cannot be empty");
            }

            ParseLabel();
            ParseAsync();

            if (index >= tokens.Count)
            {
                ThrowErrorAtEndOfLine("Statement body expected");
            }

            ParseBody();
            
            if (index < tokens.Count)
            {
                ThrowErrorOnToken("Extra tokens after statement end", tokens[index]);
            }
            
            return tokens;
        }
    }
}