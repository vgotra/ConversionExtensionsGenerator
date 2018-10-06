using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxKind;

namespace ConversionExtensionsGenerator
{
    public class ExtensionsGenerator
    {
        private readonly string _extensionsOutputNamespace;
        private readonly Assembly _sourceAssembly;
        private readonly Assembly _targetAssembly;
        private readonly string _sourceNamespace;
        private readonly string _targetNamespace;
        private readonly bool _ignoreCase;
        private readonly bool _stopOnError;

        public ExtensionsGenerator(string extensionsOutputNamespace, Assembly sourceAssembly, string sourceNamespace, Assembly targetAssembly, string targetNamespace)
        {
            //TODO: Add options and fluent rules for ignore, mapping, etc.
            _extensionsOutputNamespace = extensionsOutputNamespace;
            _sourceAssembly = sourceAssembly;
            _targetAssembly = targetAssembly;
            _sourceNamespace = sourceNamespace;
            _targetNamespace = targetNamespace;
        }

        public GenerationResult GenerateExtensionFiles()
        {
            var result = new GenerationResult();

            var mappings = GenerateClassMappings();

            if (mappings.Errors.Any())
            {
                result.Errors.AddRange(mappings.Errors);
                if (_stopOnError)
                {
                    return result;
                }
            }

            foreach (var typeMapping in mappings.Mappings)
            {
                //TODO: Remove duplicates - think about all mappings in one collection
                var sourceFieldsMappingsResult = GenerateFieldsAndPropertiesMappings(typeMapping.SourceClassType, typeMapping.TargetClassType);
                if (sourceFieldsMappingsResult.Errors.Any())
                {
                    result.Errors.AddRange(sourceFieldsMappingsResult.Errors);
                    if (_stopOnError)
                    {
                        return result;
                    }
                }

                var targetFieldsMappingsResult = GenerateFieldsAndPropertiesMappings(typeMapping.TargetClassType, typeMapping.SourceClassType);
                if (targetFieldsMappingsResult.Errors.Any())
                {
                    result.Errors.AddRange(targetFieldsMappingsResult.Errors);
                    if (_stopOnError)
                    {
                        return result;
                    }
                }

                var extensionFile = GenerateExtensionFile(typeMapping.SourceClassType, typeMapping.TargetClassType, sourceFieldsMappingsResult.Mappings, targetFieldsMappingsResult.Mappings);
                result.ExtensionFiles.Add(extensionFile);
            }

            return result;
        }

        protected static bool IsSupportedClassOrStruct(Type type, string @namespace)
        {
            if (type == null)
            {
                return false;
            }

            if (@namespace == null)
            {
                return false;
            }

            return (type.IsClass || (type.IsValueType && !type.IsEnum)) && type.Namespace == @namespace;
        }

        protected ClassMappingResult GenerateClassMappings()
        {
            //TODO: Add patterns for naming (something like Regex pattern for name mathing)
            var result = new ClassMappingResult();

            var sourceTypes = _sourceAssembly.GetExportedTypes().Where(x => IsSupportedClassOrStruct(x, _sourceNamespace)).ToList();
            var targetTypes = _targetAssembly.GetExportedTypes().Where(x => IsSupportedClassOrStruct(x, _targetNamespace)).ToList();

            foreach (var sourceType in sourceTypes)
            {

                var matchedTargetType = targetTypes.FirstOrDefault(x =>
                    x.Name.StartsWith(sourceType.Name, _ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture));

                if (matchedTargetType == null)
                {
                    result.Errors.Add(new GenerationLogItem(LogLevel.Error, $"Target type for: '{sourceType.FullName}' was not found."));
                }
                else
                {
                    if (!result.Mappings.Any(x => x.SourceClassType == sourceType && x.TargetClassType == matchedTargetType))
                    {
                        result.Mappings.Add(new ClassMappingInfo { SourceClassType = sourceType, TargetClassType = matchedTargetType });
                    }
                    else
                    {
                        result.Errors.Add(new GenerationLogItem(LogLevel.Warning, $"Possible duplicates for Source type: '{sourceType.FullName}' and Target type: '{matchedTargetType.FullName}'"));
                    }
                }
            }

            foreach (var targetType in targetTypes)
            {
                var matchedSourceType = sourceTypes.FirstOrDefault(x =>
                    x.Name.StartsWith(targetType.Name, _ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture));

                if (matchedSourceType == null)
                {
                    if (!result.Mappings.Any(x => x.SourceClassType != null && x.TargetClassType == targetType))
                    {
                        result.Errors.Add(new GenerationLogItem(LogLevel.Error, $"Source type for: '{targetType.FullName}' was not found."));
                    }
                }
                else
                {
                    if (!result.Mappings.Any(x => x.SourceClassType == matchedSourceType && x.TargetClassType == targetType))
                    {
                        result.Mappings.Add(new ClassMappingInfo { SourceClassType = matchedSourceType, TargetClassType = targetType });
                    }
                    else
                    {
                        result.Errors.Add(new GenerationLogItem(LogLevel.Warning, $"Possible duplicates for Source type: '{matchedSourceType.FullName}' and Target type: '{targetType.FullName}'"));
                    }
                }
            }

            return result;
        }

        protected ExtensionFile GenerateExtensionFile(Type sourceType, Type targetType, List<ClassMemberMappingInfo> sourceFieldsMappings, List<ClassMemberMappingInfo> targetFieldsMappings)
        {
            var classDeclaration = GenerateExtensionClass(sourceType, targetType, sourceFieldsMappings, targetFieldsMappings);

            var result = CompilationUnit()
                .WithUsings(List(new[]
                {
                    UsingDirective(GenerateNamespaceSyntax(sourceType.Namespace))
                    .WithUsingKeyword(Token(TriviaList(Comment("/* This class is auto generated */")), UsingKeyword, TriviaList())),
                    UsingDirective(GenerateNamespaceSyntax(targetType.Namespace)),
                    UsingDirective(GenerateNamespaceSyntax("System"))
                }))
                .WithMembers(classDeclaration.Syntax)
                .NormalizeWhitespace();

            return new ExtensionFile { FileName = classDeclaration.ExtensionClassName, FileSource = result.ToFullString() };
        }

        protected (string ExtensionClassName, SyntaxList<MemberDeclarationSyntax> Syntax) GenerateExtensionClass(Type sourceType, Type targetType, List<ClassMemberMappingInfo> sourceFieldsMappings, List<ClassMemberMappingInfo> targetFieldsMappings)
        {
            var sourceTypeName = sourceType.Name;
            var targetTypeName = targetType.Name;

            var sourceVariableName = char.ToLowerInvariant(sourceTypeName[0]) + sourceTypeName.Substring(1);
            var targetVariableName = char.ToLowerInvariant(targetTypeName[0]) + targetTypeName.Substring(1);

            var extensionClassName = $"{sourceTypeName}{targetTypeName}Extensions";
            var sourceMethodName = $"{sourceTypeName}To{targetTypeName}";
            var targetMethodName = $"{targetTypeName}To{sourceTypeName}";

            var outputNamespaceSyntax = GenerateNamespaceSyntax(_extensionsOutputNamespace);

            var result = SingletonList<MemberDeclarationSyntax>
            (
                NamespaceDeclaration(outputNamespaceSyntax)
                    .WithMembers(SingletonList<MemberDeclarationSyntax>
                    (
                        ClassDeclaration(extensionClassName)
                            .WithModifiers(TokenList(Token(PublicKeyword), Token(StaticKeyword), Token(PartialKeyword)))
                            .WithMembers(List(new MemberDeclarationSyntax[]
                            {
                                GenerateMethodDeclaration(sourceTypeName, sourceMethodName, targetVariableName, sourceVariableName, targetTypeName, sourceFieldsMappings),
                                GenerateMethodDeclaration(targetTypeName, targetMethodName, sourceVariableName, targetVariableName, sourceTypeName, targetFieldsMappings),
                            }))
                    ))
            );

            return (extensionClassName, result);
        }

        protected MethodDeclarationSyntax GenerateMethodDeclaration(string returnClassName, string extensionMethodName, string inputVariableName, string returnVariableName, string inputClassName,
            List<ClassMemberMappingInfo> fieldsMappings)
        {
            var ifStatement =
                IfStatement(
                    BinaryExpression(EqualsExpression, IdentifierName(inputVariableName), LiteralExpression(NullLiteralExpression)),
                    Block(ReturnStatement(LiteralExpression(NullLiteralExpression))));

            var returnVariableCreationStatement = LocalDeclarationStatement(
                VariableDeclaration(IdentifierName(returnClassName))
                    .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier(returnVariableName))
                        .WithInitializer(EqualsValueClause(ObjectCreationExpression(IdentifierName(returnClassName)).WithArgumentList(ArgumentList()))))));

            var fieldsAssignmentExpressions = GenerateFieldsAssignments(returnVariableName, inputVariableName, fieldsMappings);

            var returnStatement = ReturnStatement(IdentifierName(returnVariableName));

            var statementsList = new List<StatementSyntax>
            {
                ifStatement,
                returnVariableCreationStatement
            };

            statementsList.AddRange(fieldsAssignmentExpressions);
            statementsList.Add(returnStatement);

            //TODO: Add this for extension method signature
            return MethodDeclaration(IdentifierName(returnClassName), Identifier(extensionMethodName))
                .WithModifiers(TokenList(Token(PublicKeyword), Token(StaticKeyword)))
                .WithParameterList(ParameterList(SingletonSeparatedList(Parameter(Identifier(inputVariableName)).WithModifiers(TokenList(Token(ThisKeyword))).WithType(IdentifierName(inputClassName)))))
                .WithBody(Block(statementsList));
        }

        protected NameSyntax GenerateNamespaceSyntax(string usingNamespace)
        {
            var identifierNames = usingNamespace.Split('.').ToList();

            if (identifierNames.Count == 1)
            {
                return IdentifierName(identifierNames[0]);
            }

            if (identifierNames.Count == 2)
            {
                return QualifiedName(IdentifierName(identifierNames[0]), IdentifierName(identifierNames[1]));
            }

            var result = QualifiedName(IdentifierName(identifierNames[0]), IdentifierName(identifierNames[1]));

            for (var i = 2; i < identifierNames.Count; i++)
            {
                result = QualifiedName(result, IdentifierName(identifierNames[i]));
            }

            return result;
        }

        protected List<StatementSyntax> GenerateFieldsAssignments(string returnVariableName, string inputVariableName, List<ClassMemberMappingInfo> fieldsMapping)
        {
            var result = new List<StatementSyntax>();

            foreach (var kvp in fieldsMapping)
            {
                var syntax = ExpressionStatement(AssignmentExpression(SimpleAssignmentExpression,
                    MemberAccessExpression(SimpleMemberAccessExpression, IdentifierName(returnVariableName), IdentifierName(kvp.SourceClassMemberName)),
                    MemberAccessExpression(SimpleMemberAccessExpression, IdentifierName(inputVariableName), IdentifierName(kvp.TargetClassMemberName)))
                );

                result.Add(syntax);
            }

            return result;
        }

        protected FieldsMappingResult GenerateFieldsAndPropertiesMappings(Type sourceType, Type targetType)
        {
            //TODO: Add checks for get/set, enum, collections, etc.

            var mappingsList = new List<ClassMemberMappingInfo>();

            var sourceProperties = sourceType.GetProperties().ToList();
            var targetProperties = targetType.GetProperties().ToList();

            foreach (var sourceProperty in sourceProperties)
            {
                var targetPropertyToMap = targetProperties.FirstOrDefault(x => x.Name.Equals(sourceProperty.Name, StringComparison.InvariantCultureIgnoreCase));

                mappingsList.Add(new ClassMemberMappingInfo
                {
                    SourceClassFullName = sourceType.FullName,
                    SourceClassMemberName = sourceProperty.Name,
                    SourceClassMemberType = sourceProperty.PropertyType,
                    TargetClassFullName = targetType.FullName,
                    TargetClassMemberName = targetPropertyToMap?.Name,
                    TargetClassMemberType = targetPropertyToMap?.PropertyType
                });
            }

            foreach (var targetProperty in targetProperties)
            {
                var sourcePropertyToMap = sourceProperties.FirstOrDefault(x => x.Name.Equals(targetProperty.Name, StringComparison.InvariantCultureIgnoreCase));

                if (sourcePropertyToMap == null)
                {
                    mappingsList.Add(new ClassMemberMappingInfo
                    {
                        SourceClassFullName = sourceType.FullName,
                        SourceClassMemberName = null,
                        SourceClassMemberType = null,
                        TargetClassFullName = targetType.FullName,
                        TargetClassMemberName = targetProperty.Name,
                        TargetClassMemberType = targetProperty.PropertyType
                    });
                }
                else
                {
                    if (!mappingsList.Any(x => x.SourceClassFullName == sourceType.FullName
                        && x.SourceClassMemberName == sourcePropertyToMap.Name
                        && x.SourceClassMemberType == sourcePropertyToMap.PropertyType
                        && x.TargetClassFullName == targetType.FullName
                        && x.TargetClassMemberName == targetProperty.Name
                        && x.TargetClassMemberType == targetProperty.PropertyType))
                    {
                        mappingsList.Add(new ClassMemberMappingInfo
                        {
                            SourceClassFullName = sourceType.FullName,
                            SourceClassMemberName = sourcePropertyToMap.Name,
                            SourceClassMemberType = sourcePropertyToMap.PropertyType,
                            TargetClassFullName = targetType.FullName,
                            TargetClassMemberName = targetProperty.Name,
                            TargetClassMemberType = targetProperty.PropertyType
                        });
                    }
                }
            }

            var sourceFields = sourceType.GetFields().ToList();
            var targetFields = targetType.GetFields().ToList();

            foreach (var sourceField in sourceFields)
            {
                var targetFieldToMap = targetFields.FirstOrDefault(x => x.Name.Equals(sourceField.Name, StringComparison.InvariantCultureIgnoreCase));

                mappingsList.Add(new ClassMemberMappingInfo
                {
                    SourceClassFullName = sourceType.FullName,
                    SourceClassMemberName = sourceField.Name,
                    SourceClassMemberType = sourceField.FieldType,
                    TargetClassFullName = targetType.FullName,
                    TargetClassMemberName = targetFieldToMap?.Name,
                    TargetClassMemberType = targetFieldToMap?.FieldType
                });
            }

            foreach (var targetField in targetFields)
            {
                var sourceFieldToMap = sourceFields.FirstOrDefault(x => x.Name.Equals(targetField.Name, StringComparison.InvariantCultureIgnoreCase));

                if (sourceFieldToMap == null)
                {
                    mappingsList.Add(new ClassMemberMappingInfo
                    {
                        SourceClassFullName = sourceType.FullName,
                        SourceClassMemberName = null,
                        SourceClassMemberType = null,
                        TargetClassFullName = targetType.FullName,
                        TargetClassMemberName = targetField.Name,
                        TargetClassMemberType = targetField.FieldType
                    });
                }
                else
                {
                    if (!mappingsList.Any(x => x.SourceClassFullName == sourceType.FullName
                        && x.SourceClassMemberName == sourceFieldToMap.Name
                        && x.SourceClassMemberType == sourceFieldToMap.FieldType
                        && x.TargetClassFullName == targetType.FullName
                        && x.TargetClassMemberName == targetField.Name
                        && x.TargetClassMemberType == targetField.FieldType))
                    {
                        mappingsList.Add(new ClassMemberMappingInfo
                        {
                            SourceClassFullName = sourceType.FullName,
                            SourceClassMemberName = sourceFieldToMap.Name,
                            SourceClassMemberType = sourceFieldToMap.FieldType,
                            TargetClassFullName = targetType.FullName,
                            TargetClassMemberName = targetField.Name,
                            TargetClassMemberType = targetField.FieldType
                        });
                    }
                }
            }

            var result = new FieldsMappingResult();

            // remove unmapped fields
            var unmapped = mappingsList.Where(x => (x.TargetClassMemberName == null && x.TargetClassMemberType == null) || (x.SourceClassMemberName == null && x.SourceClassMemberType == null)).ToList();

            foreach (var item in unmapped)
            {
                result.Errors.Add(new GenerationLogItem(LogLevel.Error, $"Cannot map field/property. Source Class: '{item.SourceClassFullName}' Source Name: '{item.SourceClassMemberName}', Target Class: '{item.TargetClassFullName}', Target Name: '{item.TargetClassMemberName}'"));
            }

            mappingsList.RemoveAll(x => (x.TargetClassMemberName == null && x.TargetClassMemberType == null) || (x.SourceClassMemberName == null && x.SourceClassMemberType == null));

            //TODO: Add fluent rules, also checks for possible extensions for classes/structs (maybe also possible auto-conversions of int to long, list to ienumerable, etc.)
            // remove fields with different types 
            var differentTypes = mappingsList.Where(x => x.TargetClassMemberType != x.SourceClassMemberType).ToList();

            foreach (var item in differentTypes)
            {
                result.Errors.Add(new GenerationLogItem(LogLevel.Error, $"Cannot map field/property with different types. Source Class: '{item.SourceClassFullName}' Source Name: '{item.SourceClassMemberName}', Source Type: '{item.SourceClassMemberType.FullName}', Target Class: '{item.TargetClassFullName}', Target Name: '{item.TargetClassMemberName}', Target Type: '{item.TargetClassMemberType.FullName}'"));
            }

            mappingsList.RemoveAll(x => x.TargetClassMemberType != x.SourceClassMemberType);

            result.Mappings = mappingsList;
            return result;
        }
    }
}