using System;
using System.Reflection;

namespace Bond
{
    public class BondException : Exception
    {
        public BondException(string message ): base(message)
        {            
        }

        internal static BondException ResolveException(PropertyInfo property)
        {
            return new BondException($"Property type \"{property.PropertyType}\", used on property named \"{property.Name}\" on type \"{property.DeclaringType}\" can not be resolved. Check if the type is added to the type collection.");
        }
    }
}
