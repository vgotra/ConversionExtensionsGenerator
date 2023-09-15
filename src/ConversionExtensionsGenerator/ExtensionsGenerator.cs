using System.Collections;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxKind;

namespace ConversionExtensionsGenerator;

public class ExtensionsGenerator
{
    private readonly string _extensionsOutputNamespace;
    private readonly Assembly _sourceAssembly;
    private readonly Assembly _targetAssembly;
    private readonly string _sourceNamespace;
    private readonly string _targetNamespace;
    private readonly bool _ignoreCase;
    private readonly bool _stopOnError;

    public ExtensionsGenerator(string extensionsOutputNamespace, Assembly sourceAssembly, string sourceNamespace,
        Assembly targetAssembly, string targetNamespace)
    {
        //TODO: Add options and fluent rules for ignore, mapping, etc.
        //TODO: Check for bugs in generation and missed fields
        //TODO: Fix tech debt and improve code quality
        //TODO: Make full analysis of structures before generation
        _extensionsOutputNamespace = extensionsOutputNamespace;
        _sourceAssembly = sourceAssembly;
        _targetAssembly = targetAssembly;
        _sourceNamespace = sourceNamespace;
        _targetNamespace = targetNamespace;
    }

    public GenerationResult GenerateExtensionFiles()
    {
        var result = new GenerationResult();

        var classMappings = GenerateClassMappings();

        if (classMappings.Errors.Any())
        {
            result.Errors.AddRange(classMappings.Errors);
            if (_stopOnError)
            {
                return result;
            }
        }

        foreach (var typeMapping in classMappings.Mappings)
        {
            //TODO: Remove duplicates - think about all mappings in one collection
            var sourceFieldsMappingsResult = GenerateFieldsAndPropertiesMappings(typeMapping.SourceClassType,
                typeMapping.TargetClassType, classMappings.Mappings);
            if (sourceFieldsMappingsResult.Errors.Any())
            {
                result.Errors.AddRange(sourceFieldsMappingsResult.Errors);
                if (_stopOnError)
                {
                    return result;
                }
            }

            var targetFieldsMappingsResult = GenerateFieldsAndPropertiesMappings(typeMapping.TargetClassType,
                typeMapping.SourceClassType, classMappings.Mappings);
            if (targetFieldsMappingsResult.Errors.Any())
            {
                result.Errors.AddRange(targetFieldsMappingsResult.Errors);
                if (_stopOnError)
                {
                    return result;
                }
            }

            var extensionFile = GenerateExtensionFile(typeMapping.SourceClassType, typeMapping.TargetClassType,
                sourceFieldsMappingsResult.Mappings, targetFieldsMappingsResult.Mappings);
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

    protected static bool IsClassOrStruct(Type type)
    {
        if (type == null)
        {
            return false;
        }

        return (type.IsClass || (type.IsValueType && !type.IsEnum)) &&
               (!type.IsPrimitive && type != typeof(string) && type != typeof(Guid));
    }

    protected static bool IsEnum(Type type)
    {
        if (type == null)
        {
            return false;
        }

        return type.IsEnum;
    }

    protected static bool IsGenericCollection(Type type)
    {
        if (type == null)
        {
            return false;
        }

        return type.GetInterface(nameof(IEnumerable)) != null && type != typeof(string) && type.IsGenericType;
    }

    protected ClassMappingResult GenerateClassMappings()
    {
        //TODO: Add patterns for naming (something like Regex pattern for name mathing)
        var result = new ClassMappingResult();

        var sourceTypes = _sourceAssembly.GetExportedTypes().Where(x => IsSupportedClassOrStruct(x, _sourceNamespace)).ToList();
        var targetTypes = _targetAssembly.GetExportedTypes().Where(x => IsSupportedClassOrStruct(x, _targetNamespace)).ToList();

        foreach (var sourceType in sourceTypes)
        {
            var matchedTargetType = targetTypes.FirstOrDefault(x => x.Name.StartsWith(sourceType.Name, _ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture));
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
            var matchedSourceType = sourceTypes.FirstOrDefault(x => x.Name.StartsWith(targetType.Name, _ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture));

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

    protected ExtensionFile GenerateExtensionFile(Type sourceType, Type targetType,
        List<ClassMemberMappingInfo> sourceFieldsMappings, List<ClassMemberMappingInfo> targetFieldsMappings)
    {
        var classDeclaration =
            GenerateExtensionClass(sourceType, targetType, sourceFieldsMappings, targetFieldsMappings);

        var result = CompilationUnit()
            .WithUsings(List(new[]
            {
                UsingDirective(GenerateNamespaceSyntax("System")).WithUsingKeyword(
                    Token(TriviaList(Comment("/* This class is auto generated */")), UsingKeyword, TriviaList())),
                UsingDirective(GenerateNamespaceSyntax("System.Collections.Generic")),
                UsingDirective(GenerateNamespaceSyntax("System.Linq")),
                UsingDirective(GenerateNamespaceSyntax(sourceType.Namespace)),
                UsingDirective(GenerateNamespaceSyntax(targetType.Namespace)),
                UsingDirective(GenerateNamespaceSyntax(_extensionsOutputNamespace))

            }))
            .WithMembers(classDeclaration.Syntax)
            .NormalizeWhitespace();

        return new ExtensionFile
            { FileName = classDeclaration.ExtensionClassName, FileSource = result.ToFullString() };
    }

    protected (string ExtensionClassName, SyntaxList<MemberDeclarationSyntax> Syntax) GenerateExtensionClass(
        Type sourceType, Type targetType, List<ClassMemberMappingInfo> sourceFieldsMappings,
        List<ClassMemberMappingInfo> targetFieldsMappings)
    {
        var sourceTypeName = sourceType.Name;
        var targetTypeName = targetType.Name;

        var sourceVariableName = char.ToLowerInvariant(sourceTypeName[0]) + sourceTypeName.Substring(1);
        var targetVariableName = char.ToLowerInvariant(targetTypeName[0]) + targetTypeName.Substring(1);

        var extensionClassName = $"{sourceTypeName}{targetTypeName}Extensions";
        var sourceMethodName = $"{sourceTypeName}To{targetTypeName}";
        var targetMethodName = $"{targetTypeName}To{sourceTypeName}";

        var outputNamespaceSyntax = GenerateNamespaceSyntax(_extensionsOutputNamespace);

        var methodsList = new List<MemberDeclarationSyntax>
        {
            GenerateMethodDeclaration(sourceTypeName, targetMethodName, targetVariableName, sourceVariableName, targetTypeName, sourceFieldsMappings),
            GenerateCollectionMethodDeclaration(sourceTypeName, targetMethodName, targetVariableName, sourceVariableName, targetTypeName, sourceFieldsMappings),
            GenerateMethodDeclaration(targetTypeName, sourceMethodName, sourceVariableName, targetVariableName, sourceTypeName, targetFieldsMappings),
            GenerateCollectionMethodDeclaration(targetTypeName, sourceMethodName, sourceVariableName, targetVariableName, sourceTypeName, targetFieldsMappings),
        };

        var result = SingletonList<MemberDeclarationSyntax>
        (
            NamespaceDeclaration(outputNamespaceSyntax)
                .WithMembers(SingletonList<MemberDeclarationSyntax>
                (
                    ClassDeclaration(extensionClassName)
                        .WithModifiers(TokenList(Token(PublicKeyword), Token(StaticKeyword), Token(PartialKeyword)))
                        .WithMembers(List(methodsList))
                ))
        );

        return (extensionClassName, result);
    }

    protected MethodDeclarationSyntax GenerateMethodDeclaration(string returnClassName, string extensionMethodName,
        string inputVariableName, string returnVariableName, string inputClassName,
        List<ClassMemberMappingInfo> fieldsMappings)
    {
        var ifStatement =
            IfStatement(
                BinaryExpression(EqualsExpression, IdentifierName(inputVariableName),
                    LiteralExpression(NullLiteralExpression)),
                Block(ReturnStatement(LiteralExpression(NullLiteralExpression))));

        var returnVariableCreationStatement = LocalDeclarationStatement(
            VariableDeclaration(IdentifierName(returnClassName))
                .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier(returnVariableName))
                    .WithInitializer(EqualsValueClause(ObjectCreationExpression(IdentifierName(returnClassName))
                        .WithArgumentList(ArgumentList()))))));

        var fieldsAssignmentExpressions =
            GenerateFieldsAssignments(returnVariableName, inputVariableName, fieldsMappings);

        var returnStatement = ReturnStatement(IdentifierName(returnVariableName));

        var statementsList = new List<StatementSyntax>
        {
            ifStatement,
            returnVariableCreationStatement
        };

        statementsList.AddRange(fieldsAssignmentExpressions);
        statementsList.Add(returnStatement);

        return MethodDeclaration(IdentifierName(returnClassName), Identifier(extensionMethodName))
            .WithModifiers(TokenList(Token(PublicKeyword), Token(StaticKeyword)))
            .WithParameterList(ParameterList(SingletonSeparatedList(Parameter(Identifier(inputVariableName))
                .WithModifiers(TokenList(Token(ThisKeyword))).WithType(IdentifierName(inputClassName)))))
            .WithBody(Block(statementsList));
    }

    protected MethodDeclarationSyntax GenerateCollectionMethodDeclaration(string returnClassName, string extensionMethodName, string inputVariableName, string returnVariableName, string inputClassName,
        List<ClassMemberMappingInfo> fieldsMappings)
    {
        //TODO: Add option for names templates
        var ifStatement = IfStatement(
            BinaryExpression(EqualsExpression, IdentifierName($"{inputVariableName}Collection"), LiteralExpression(NullLiteralExpression)),
            Block(SingletonList<StatementSyntax>(ReturnStatement(LiteralExpression(NullLiteralExpression)))));

        var returnCollectionCreationStatement = LocalDeclarationStatement(
            VariableDeclaration(GenericName(Identifier("List")).WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName(returnClassName)))))
                .WithVariables(SingletonSeparatedList(
                    VariableDeclarator(Identifier($"{returnVariableName}Collection"))
                        .WithInitializer(EqualsValueClause(InvocationExpression(
                            MemberAccessExpression(SimpleMemberAccessExpression,
                                InvocationExpression(MemberAccessExpression(SimpleMemberAccessExpression, IdentifierName($"{inputVariableName}Collection"), IdentifierName("Select")))
                                    .WithArgumentList(ArgumentList(SingletonSeparatedList(
                                        Argument(SimpleLambdaExpression(
                                            Parameter(Identifier("x")),
                                            InvocationExpression(MemberAccessExpression(SimpleMemberAccessExpression, IdentifierName("x"), IdentifierName($"{inputClassName}To{returnClassName}")))
                                        ))))),
                                IdentifierName("ToList"))))))));

        var returnStatement = ReturnStatement(IdentifierName($"{returnVariableName}Collection"));

        var statementsList = new List<StatementSyntax>
        {
            ifStatement,
            returnCollectionCreationStatement
        };

        statementsList.Add(returnStatement);

        var result = MethodDeclaration(GenericName(Identifier("List"))
                    .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName(returnClassName)))),
                Identifier($"{inputClassName}CollectionTo{returnClassName}Collection"))
            .WithModifiers(TokenList(new[] { Token(PublicKeyword), Token(StaticKeyword) }))
            .WithParameterList(ParameterList(SingletonSeparatedList(Parameter(Identifier($"{inputVariableName}Collection"))
                .WithModifiers(TokenList(Token(ThisKeyword)))
                .WithType(GenericName(Identifier(nameof(IEnumerable)))
                    .WithTypeArgumentList(TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName(inputClassName))))))))
            .WithBody(Block(statementsList));

        return result;
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

    protected List<StatementSyntax> GenerateFieldsAssignments(string returnVariableName, string inputVariableName,
        List<ClassMemberMappingInfo> fieldsMappings)
    {
        //TODO: Also dictionaries and other (inheritance), cases int -< enum or enum -> int
        var statementSyntaxList = new List<StatementSyntax>();

        foreach (var fieldMapping in fieldsMappings)
        {
            ExpressionSyntax expression = null;

            if (IsEnum(fieldMapping.TargetClassMemberType))
            {
                expression = CastExpression(IdentifierName(fieldMapping.SourceClassMemberType.Name),
                    MemberAccessExpression(SimpleMemberAccessExpression, IdentifierName(inputVariableName),
                        IdentifierName(fieldMapping.TargetClassMemberName)));
            }
            else if (IsClassOrStruct(fieldMapping.TargetClassMemberType) &&
                     !IsGenericCollection(fieldMapping.TargetClassMemberType))
            {
                expression = InvocationExpression(MemberAccessExpression(SimpleMemberAccessExpression,
                    MemberAccessExpression(SimpleMemberAccessExpression,
                        IdentifierName(inputVariableName), IdentifierName(fieldMapping.TargetClassMemberName)),
                    IdentifierName(
                        $"{fieldMapping.TargetClassMemberType.Name}To{fieldMapping.SourceClassMemberType.Name}")));
            }
            else if (IsGenericCollection(fieldMapping.TargetClassMemberType))
            {
                //TODO: Additional check for dictionaries
                var targetTypeName = fieldMapping.TargetClassMemberType.GenericTypeArguments.First().Name;
                var sourceTypeName = fieldMapping.SourceClassMemberType.GenericTypeArguments.First().Name;

                expression = InvocationExpression(MemberAccessExpression(SimpleMemberAccessExpression,
                    MemberAccessExpression(SimpleMemberAccessExpression,
                        IdentifierName(inputVariableName), IdentifierName(fieldMapping.TargetClassMemberName)),
                    IdentifierName($"{targetTypeName}CollectionTo{sourceTypeName}Collection")));
            }
            else
            {
                expression = MemberAccessExpression(SimpleMemberAccessExpression, IdentifierName(inputVariableName),
                    IdentifierName(fieldMapping.TargetClassMemberName));
            }

            var syntax = ExpressionStatement(AssignmentExpression(SimpleAssignmentExpression,
                MemberAccessExpression(SimpleMemberAccessExpression, IdentifierName(returnVariableName),
                    IdentifierName(fieldMapping.SourceClassMemberName)),
                expression
            ));

            statementSyntaxList.Add(syntax);
        }

        return statementSyntaxList;
    }

    protected ClassMemberMappingResult GenerateFieldsAndPropertiesMappings(Type sourceType, Type targetType, List<ClassMappingInfo> classMappingInfos)
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
                SourceClassType = sourceType,
                SourceClassMemberName = sourceProperty.Name,
                SourceClassMemberType = sourceProperty.PropertyType,
                TargetClassType = targetType,
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
                    SourceClassType = sourceType,
                    SourceClassMemberName = null,
                    SourceClassMemberType = null,
                    TargetClassType = targetType,
                    TargetClassMemberName = targetProperty.Name,
                    TargetClassMemberType = targetProperty.PropertyType
                });
            }
            else
            {
                if (!mappingsList.Any(x => x.SourceClassType == sourceType
                                           && x.SourceClassMemberName == sourcePropertyToMap.Name
                                           && x.SourceClassMemberType == sourcePropertyToMap.PropertyType
                                           && x.TargetClassType == targetType
                                           && x.TargetClassMemberName == targetProperty.Name
                                           && x.TargetClassMemberType == targetProperty.PropertyType))
                {
                    mappingsList.Add(new ClassMemberMappingInfo
                    {
                        SourceClassType = sourceType,
                        SourceClassMemberName = sourcePropertyToMap.Name,
                        SourceClassMemberType = sourcePropertyToMap.PropertyType,
                        TargetClassType = targetType,
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
                SourceClassType = sourceType,
                SourceClassMemberName = sourceField.Name,
                SourceClassMemberType = sourceField.FieldType,
                TargetClassType = targetType,
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
                    SourceClassType = sourceType,
                    SourceClassMemberName = null,
                    SourceClassMemberType = null,
                    TargetClassType = targetType,
                    TargetClassMemberName = targetField.Name,
                    TargetClassMemberType = targetField.FieldType
                });
            }
            else
            {
                if (!mappingsList.Any(x => x.SourceClassType == sourceType
                                           && x.SourceClassMemberName == sourceFieldToMap.Name
                                           && x.SourceClassMemberType == sourceFieldToMap.FieldType
                                           && x.TargetClassType == targetType
                                           && x.TargetClassMemberName == targetField.Name
                                           && x.TargetClassMemberType == targetField.FieldType))
                {
                    mappingsList.Add(new ClassMemberMappingInfo
                    {
                        SourceClassType = sourceType,
                        SourceClassMemberName = sourceFieldToMap.Name,
                        SourceClassMemberType = sourceFieldToMap.FieldType,
                        TargetClassType = targetType,
                        TargetClassMemberName = targetField.Name,
                        TargetClassMemberType = targetField.FieldType
                    });
                }
            }
        }

        var baseResult = AnalyzeClassMemberMappings(mappingsList, classMappingInfos);

        var result = new ClassMemberMappingResult();
        result.Errors.AddRange(baseResult.Errors);
        result.Mappings = mappingsList;

        return result;
    }

    private static BaseResult AnalyzeClassMemberMappings(List<ClassMemberMappingInfo> mappingsList, List<ClassMappingInfo> classMappingInfos)
    {
        var baseResult = new BaseResult();

        // remove unmapped fields
        var unmapped = mappingsList.Where(x =>
            (x.TargetClassMemberName == null && x.TargetClassMemberType == null) ||
            (x.SourceClassMemberName == null && x.SourceClassMemberType == null)).ToList();

        foreach (var item in unmapped)
        {
            baseResult.Errors.Add(new GenerationLogItem(LogLevel.Error,
                $"Cannot map field/property. Source Class: '{item.SourceClassType.FullName}' Source Name: '{item.SourceClassMemberName}', Target Class: '{item.TargetClassType.FullName}', Target Name: '{item.TargetClassMemberName}'"));
        }

        unmapped.ForEach(x => mappingsList.Remove(x));

        //TODO: Add fluent rules, also checks for possible extensions for classes/structs (maybe also possible auto-conversions of int to long, list to ienumerable, etc.)
        // remove fields with different types 
        var differentTypes = mappingsList.Where(x =>
                x.TargetClassMemberType != x.SourceClassMemberType &&
                (!classMappingInfos.Any(y =>
                    (y.TargetClassType == x.TargetClassType && y.SourceClassType == x.SourceClassType) ||
                    (y.TargetClassType == x.SourceClassType && y.SourceClassType == x.TargetClassType))
                ))
            .ToList();

        foreach (var item in differentTypes)
        {
            baseResult.Errors.Add(new GenerationLogItem(LogLevel.Error,
                $"Cannot map field/property with different types. Source Class: '{item.SourceClassType.FullName}' Source Name: '{item.SourceClassMemberName}', Source Type: '{item.SourceClassMemberType.FullName}', Target Class: '{item.TargetClassType.FullName}', Target Name: '{item.TargetClassMemberName}', Target Type: '{item.TargetClassMemberType.FullName}'"));
        }

        differentTypes.ForEach(x => mappingsList.Remove(x));

        return baseResult;
    }
}