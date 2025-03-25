using UnityEngine;

namespace JSM.JSME
{
    public class JsmeCompare
    {
        public static bool Equal(object a, object b)
        {
            if (a is int i1)
            {
                if (b is int i2)
                {
                    return i1 == i2;   
                }
                
                if (b is float f2)
                {
                    return Mathf.Approximately(i1, f2);
                }
                return false;
            }
            
            if (a is float f1)
            {
                if (b is float f2)
                {
                    return Mathf.Approximately(f1, f2);
                }
                
                if (b is int i2)
                {
                    return Mathf.Approximately(f1, i2);
                }

                return false;
            }

            if (a is string str1)
            {
                if (b is string str2)
                {
                    return str1 == str2;
                }

                return false;
            }

            return a == b;
        }
        
    }
}