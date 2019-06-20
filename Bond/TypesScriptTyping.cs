using System;
using System.IO;
using System.Linq;

namespace Bond
{
    public class TypesScriptTyping
    {
        public NamingConvention NamingConvention { get; set; } = NamingConvention.CamelCase;

        public ScriptGenerator Formatter { get; set; } = new ScriptGenerator();

        public TypeResolver TypeResolver { get; set; } = new TypeResolver();

        public void Generate(TypeCollection types, string outputFilename)
        {           
            TypeResolver.TypesToConvert = types.Types;
            TypeResolver.NamingConvention = NamingConvention;

            var namespaceGroups = types.Types
                .GroupBy(@class => @class.IsEnum ? NamingConvention.GetEnumNamespaceName(@class) : NamingConvention.GetClassNamespaceName(@class))
                .Select(group => new { NamespaceName = group.Key, Types = group.Select(type => type).ToArray() })
                .ToArray();

            Formatter.WriteFileStart();

            var headerLineCount = Formatter.Lines.Count;

            foreach (var namespaceGroup in namespaceGroups)
            {
                GenerateNamespace(namespaceGroup.NamespaceName, namespaceGroup.Types);
            }

            Formatter.WriteFileEnd();

            if (File.Exists(outputFilename))
            {
                var existingFileContent = File.ReadAllLines(outputFilename);
                var newFileContentExcludingHeader = Formatter.Lines.Skip(headerLineCount);
                var existingFileContentExcludingHeader = existingFileContent.Skip(headerLineCount);

                if (newFileContentExcludingHeader.SequenceEqual(existingFileContentExcludingHeader))
                {
                    return;
                }
            }

            File.WriteAllLines(outputFilename, Formatter.Lines);
        }

        private void GenerateNamespace(string namespaceName, Type[] types)
        {
            TypeResolver.CurrentNamespaceName = namespaceName;

            Formatter.WriteNamespaceStart(namespaceName);

            foreach (var type in types)
            {
                if (type.IsClass)
                {
                    GenerateClass(type);
                }

                if (type.IsEnum)
                {
                    GenerateEnum(type);
                }
            }

            Formatter.WriteNamespaceEnd();
        }

        private void GenerateClass(Type @class)
        {
            Formatter.WriteClassStart(NamingConvention.GetClassName(@class));

            foreach (var property in @class.GetProperties().Where(property => property.CanRead))
            {
                var classPropertyName = NamingConvention.GetClassPropertyName(property);
                var propertyTypeName = TypeResolver.GetTypeName(property.PropertyType);

                if (propertyTypeName == null)
                {
                    throw BondException.ResolveException(property);
                }

                Formatter.WriteClassProperty(classPropertyName, propertyTypeName);
            }

            Formatter.WriteClassEnd();
        }

        private void GenerateEnum(Type @enum)
        {
            Formatter.WriteEnumStart(NamingConvention.GetEnumName(@enum));

            foreach (var constantName in Enum.GetNames(@enum))
            {
                var enumConstantName = NamingConvention.GetEnumConstantName(constantName);

                Formatter.WriteEnumConstant(enumConstantName, (Enum)Enum.Parse(@enum, enumConstantName));
            }

            Formatter.WriteEnumEnd();
        }
    }
}