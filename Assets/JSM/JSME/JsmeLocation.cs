using System;

namespace JSM.JSME
{
    public class JsmeLocation
    {
        public readonly int Line = 0;
        public readonly int Column = 0;
        
        public JsmeLocation(int line, int column)
        {
            if (line < 1)
            {
                throw new ArgumentException("Line cannot be < 1");
            }
            
            if (column < 1)
            {
                throw new ArgumentException("Column cannot be < 1");
            }

            Line = line;
            Column = column;
        }

        public override bool Equals(object obj)
        {
            return obj is JsmeLocation output && output.Line == Line && output.Column == Column;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Line, Column);
        }
    }
}