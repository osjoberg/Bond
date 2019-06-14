using System;
using System.Linq;
using System.Reflection;

namespace Bond
{
    public class NamingConvention
    {
        public static readonly NamingConvention CamelCase = new NamingConvention(true);
        public static readonly NamingConvention PascalCase = new NamingConvention(false);

        private readonly bool camelCase;

        private NamingConvention(bool camelCase)
        {
            this.camelCase = camelCase;
        }

        public virtual string GetClassNamespaceName(Type type)
        {
            return type.Namespace;
        }

        public virtual string GetClassName(Type type)
        {
            return type.Name;
        }

        public virtual string GetEnumNamespaceName(Type type)
        {
            return type.Namespace;
        }

        public virtual string GetEnumName(Type type)
        {
            return type.Name;
        }

        public virtual string GetClassPropertyName(PropertyInfo propertyInfo)
        {
            if (camelCase)
            {
                return char.ToLowerInvariant(propertyInfo.Name[0]) + propertyInfo.Name.Substring(1);
            }

            return propertyInfo.Name;
        }

        public virtual string GetEnumConstantName(string constantName)
        {
            return constantName;
        }

        public virtual string GetClassPropertyNamespaceName(string currentNamespace, Type type)
        {
            var typeNamespaceSplit = type.Namespace.Split('.');
            var currentNamespaceSplit = currentNamespace.Split('.');

            var maxCommonNamespacePartCount = Math.Min(typeNamespaceSplit.Length, currentNamespaceSplit.Length);

            var commonNamespaceParts = Enumerable
                .Range(0, maxCommonNamespacePartCount)
                .SkipWhile(i => currentNamespaceSplit[i] == typeNamespaceSplit[i])
                .Select(i => typeNamespaceSplit[i]);

            return string.Join(".", commonNamespaceParts);
        }
    }
}
