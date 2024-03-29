﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bond
{
    public class TypeResolver
    {
        private readonly Dictionary<Type, string> definedTypeNames = new Dictionary<Type, string>
        {
            { typeof(bool), "boolean" },
            { typeof(DateTime), "string" },
            { typeof(TimeSpan), "string" },
            { typeof(string), "string | null" },
            { typeof(char), "string" },
            { typeof(sbyte), "number" },
            { typeof(short), "number" },
            { typeof(int), "number" },
            { typeof(long), "number" },
            { typeof(byte), "number" },
            { typeof(ushort), "number" },
            { typeof(uint), "number" },
            { typeof(ulong), "number" },
            { typeof(float), "number" },
            { typeof(double), "number" },
            { typeof(decimal), "number" },
            { typeof(object), "any" },
        };

        internal HashSet<Type> TypesToConvert { get; set; }

        internal NamingConvention NamingConvention { get; set; }

        internal string CurrentNamespaceName { get; set; }

        public virtual string GetTypeName(Type type)
        {
            return 
                GetDefinedTypeName(type) ?? 
                GetNullableTypeName(type) ?? 
                GetDictionaryTypeName(type) ??
                GetArrayTypeName(type) ??
                GetEnumerableTypeName(type) ??
                GetConvertedTypeName(type);
        }

        public virtual string GetEnumerableTypeName(Type type)
        {
            var genericEnumerableType = type.GetInterface("IEnumerable`1");
            if (genericEnumerableType != null)
            {
                var typeName = GetTypeName(genericEnumerableType.GetGenericArguments()[0]);
                if (typeName == null)
                {
                    return null;
                }

                return WrapTypeName(typeName) + "[]";
            }

            var isEnumerable = type.IsAssignableFrom(typeof(IEnumerable));
            if (isEnumerable)
            {
                return "any[]";
            }

            return null;
        }

        public virtual string GetDefinedTypeName(Type type)
        {
            return definedTypeNames.TryGetValue(type, out var resolvedType) ? resolvedType : null;
        }

        public virtual string GetNullableTypeName(Type type)
        {
            var nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType == null)
            {
                return null;
            }

            var typeName = GetTypeName(nullableType);
            if (typeName == null)
            {
                return null;
            }

            return typeName + " | null";
        }

        public virtual string GetDictionaryTypeName(Type type)
        {
            var genericDictionaryType = type.GetInterface("IDictionary`2");
            if (genericDictionaryType != null)
            {
                var genericArguments = genericDictionaryType.GetGenericArguments();
                var typeName = GetTypeName(genericArguments[1]);
                if (typeName == null)
                {
                    return null;
                }

                var valueTypeName = WrapTypeName(typeName);

                return "{[key: string]: " + valueTypeName + "}";
            }

            var isDictionary = type.IsAssignableFrom(typeof(IDictionary));
            if (isDictionary)
            {
                return "{[key: string]: any}";
            }

            return null;
        }

        public virtual string GetArrayTypeName(Type type)
        {
            if (type.IsArray == false)
            {
                return null;
            }

            var typeName = GetTypeName(type.GetElementType());
            if (typeName == null)
            {
                return null;
            }

            return WrapTypeName(typeName) + string.Concat(Enumerable.Repeat("[]", type.GetArrayRank()));
        }

        public virtual string GetConvertedTypeName(Type type)
        {
            if (TypesToConvert.Contains(type) == false)
            {
                return null;
            }

            var namespaceName = NamingConvention.GetClassPropertyNamespaceName(CurrentNamespaceName, type);

            return (namespaceName == "" ? "" : namespaceName + ".") + type.Name;
        }

        public virtual void DefineType(Type type, string typeName)
        {
            definedTypeNames[type] = typeName;
        }

        private string WrapTypeName(string resolved)
        {
            return resolved.Contains("|") ? $"({resolved})" : resolved;
        }
    }
}
