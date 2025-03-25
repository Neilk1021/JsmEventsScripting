using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace JSM.JSME
{
    public class JsmeObject: MonoBehaviour
    {
        [SerializeField] private string id;

        public string GetId()
        {
            return id;
        }

        public void SetId(string newId)
        {
            id = newId;
        }
        
        public Delegate GetDelegate(string methodName)
        {
            //throw new NotImplementedException("Not Yet written");
            Component[] components = gameObject.GetComponents<Component>();

            foreach (var component in components)
            {
                MethodInfo[] methods = component.GetType().GetMethods(BindingFlags.Public| BindingFlags.NonPublic | BindingFlags.Instance);
                
                foreach (MethodInfo method in methods)
                {
                    if (method.Name == methodName)
                    {
                        return Delegate.CreateDelegate(Expression.GetDelegateType(
                            (from parameter in method.GetParameters() select parameter.ParameterType)
                            .Concat(new[] { method.ReturnType })
                            .ToArray()), component, method);
                    }
                }
            }

            throw new ArgumentException("Function does not exist in object");
        }
    }
}