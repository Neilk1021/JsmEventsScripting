using System;
using System.Collections.Generic;

namespace JSM.JSME
{
    public enum JsmeTokenCategory
    {
        ComparisonOperator = 1,
        Identifier = 2,
        Keyword = 3,
        LiteralValue = 4,
        Punctuation = 5,
    }

    public class JsmeTokenKind
    {
        public readonly string Name;
        public readonly JsmeTokenCategory Category;
        
        public JsmeTokenKind(){}
        
        public JsmeTokenKind(string name, JsmeTokenCategory category)
        {
            Name = name;
            Category = category;
        }
    }

    public static class JsmeTokenKinds
    {
        public static readonly JsmeTokenKind Add = new JsmeTokenKind("ADD", JsmeTokenCategory.Keyword);
        public static readonly JsmeTokenKind Colon = new JsmeTokenKind("Colon", JsmeTokenCategory.Punctuation);
        public static readonly JsmeTokenKind Div = new JsmeTokenKind("DIV", JsmeTokenCategory.Keyword);
        public static readonly JsmeTokenKind Dot = new JsmeTokenKind("Dot", JsmeTokenCategory.Punctuation);
        public static readonly JsmeTokenKind End = new JsmeTokenKind("END", JsmeTokenCategory.Keyword);
        public static readonly JsmeTokenKind Equal = new JsmeTokenKind("Equal", JsmeTokenCategory.ComparisonOperator);
        public static readonly JsmeTokenKind Goto = new JsmeTokenKind("GOTO", JsmeTokenCategory.Keyword);
        public static readonly JsmeTokenKind GreaterThan = new JsmeTokenKind("GreaterThan", JsmeTokenCategory.ComparisonOperator);
        public static readonly JsmeTokenKind GreaterThanOrEqual = new JsmeTokenKind("GreaterThanOrEqual", JsmeTokenCategory.ComparisonOperator);
        public static readonly JsmeTokenKind Identifier = new JsmeTokenKind("Identifier", JsmeTokenCategory.Identifier);
        public static readonly JsmeTokenKind If = new JsmeTokenKind("IF", JsmeTokenCategory.Keyword);
        public static readonly JsmeTokenKind LessThan = new JsmeTokenKind("LessThan", JsmeTokenCategory.ComparisonOperator);
        public static readonly JsmeTokenKind LessThanOrEqual = new JsmeTokenKind("LessThanOrEqual", JsmeTokenCategory.ComparisonOperator);
        public static readonly JsmeTokenKind LiteralFloat = new JsmeTokenKind("LiteralFloat", JsmeTokenCategory.LiteralValue);
        public static readonly JsmeTokenKind LiteralInt = new JsmeTokenKind("LiteralInt", JsmeTokenCategory.LiteralValue);
        public static readonly JsmeTokenKind LiteralStr = new JsmeTokenKind("LiteralStr", JsmeTokenCategory.LiteralValue);
        public static readonly JsmeTokenKind Multiply = new JsmeTokenKind("MULT", JsmeTokenCategory.Keyword);
        public static readonly JsmeTokenKind NotEqual = new JsmeTokenKind("NotEqual", JsmeTokenCategory.ComparisonOperator);
        public static readonly JsmeTokenKind Async = new JsmeTokenKind("ASYNC", JsmeTokenCategory.Keyword);
        public static readonly JsmeTokenKind Await = new JsmeTokenKind("AWAIT", JsmeTokenCategory.Keyword);
        public static readonly JsmeTokenKind OnComplete = new JsmeTokenKind("ONCOMPLETE", JsmeTokenCategory.Keyword);
        public static readonly JsmeTokenKind Set = new JsmeTokenKind("SET", JsmeTokenCategory.Keyword);
        public static readonly JsmeTokenKind Run = new JsmeTokenKind("RUN", JsmeTokenCategory.Keyword);
        public static readonly JsmeTokenKind Sub = new JsmeTokenKind("SUB", JsmeTokenCategory.Keyword);
        public static readonly JsmeTokenKind LeftParenthesis = new JsmeTokenKind("(", JsmeTokenCategory.Punctuation);
        public static readonly JsmeTokenKind RightParenthesis = new JsmeTokenKind(")", JsmeTokenCategory.Punctuation);
        public static readonly JsmeTokenKind Print = new JsmeTokenKind("PRINT", JsmeTokenCategory.Keyword);

        public static JsmeTokenKind[] TokenKinds = new JsmeTokenKind[]
        {
            Add,
            Colon,
            Div,
            Dot,
            End,
            Equal,
            Goto,
            GreaterThan,
            GreaterThanOrEqual,
            Identifier,
            If,
            LessThan,
            LessThanOrEqual,
            LiteralFloat,
            LiteralInt,
            LiteralStr,
            Multiply,
            NotEqual,
            Async,
            Await,
            OnComplete,
            Set,
            Run,
            Sub,
            LeftParenthesis,
            RightParenthesis,
            Print
        };
    }
    
    public class JsmeToken
    {
        private readonly JsmeTokenKind _kind;
        private readonly string _text;
        private readonly JsmeLocation _location;
        private readonly object _value;
        
        public JsmeToken(JsmeTokenKind kind, string text, JsmeLocation location, object value)
        {
            _kind = kind;
            _text = text;
            _location = location;
            _value = value;
        }
        
        public JsmeTokenKind Kind() => _kind;
        public string Text() => _text;
        public JsmeLocation Location() => _location;
        public object Value() => _value;
        
        public override bool Equals(object obj)
        {
            if (obj is JsmeToken other)
            {
                return _kind == other._kind &&
                       _text == other._text &&
                       _location.Equals(other._location) &&
                       Equals(_value, other._value);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_kind, _text, _location, _value);
        }
    }
}