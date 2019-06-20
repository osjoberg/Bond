using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Bond
{
    public class TypeCollection
    {
        internal readonly HashSet<Type> Types = new HashSet<Type>();

        public void AddType<T>()
        {
            AddType(typeof(T));
        }

        public void AddType(Type type)
        {
            Argument.IsNotNull(type, nameof(type));

            if (Types.Contains(type))
            {
                throw new ArgumentException($"Type {type} is already in the type collection.", nameof(type));
            }

            Types.Add(type);
        }

        public void RemoveType<T>()
        {
            RemoveType(typeof(T));
        }

        public void RemoveType(Type type)
        {
            Argument.IsNotNull(type, nameof(type));

            if (Types.Contains(type) == false)
            {
                throw new ArgumentException($"Type {type} is not in the type collection.", nameof(type));
            }

            Types.Remove(type);
        }

        public void AddNamespace(string @namespace, bool recursive = true)
        {
            AddNamespace(Assembly.GetCallingAssembly(), @namespace, recursive);
        }

        public void AddNamespace(Assembly assembly, string @namespace, bool recursive = true)
        {
            var query = assembly.GetTypes()
                .Where(type => type.Namespace != null)
                .Where(type => type.IsClass || type.IsEnum)
                .Where(type => Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute)) == false);

            query = recursive ? query.Where(type => type.Namespace.StartsWith(@namespace)) : query.Where(type => type.Namespace == @namespace);

            foreach (var result in query)
            {
                if (Types.Contains(result))
                {
                    throw new ArgumentException($"Type {result} is already in the type collection.", nameof(@namespace));
                }

                Types.Add(result);
            }
        }
    }
}
