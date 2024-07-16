using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace StructPolymorphismGenerator.Sample;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
[Generator]
public class AbstractStructGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var structDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                (node, _) => node is StructDeclarationSyntax sd && sd.AttributeLists.Any(al => al.Attributes.Any(a => a.Name.ToString() == "AbstractStruct")),
                (context, _) => (StructDeclarationSyntax)context.Node)
            .Where(s => s is not null)
            .Collect();

        var interfaceDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                (node, _) => node is InterfaceDeclarationSyntax id && id.AttributeLists.Any(al => al.Attributes.Any(a => a.Name.ToString() == "AbstractStructGenerator")),
                (context, _) => (InterfaceDeclarationSyntax)context.Node)
            .Where(s => s is not null)
            .Collect();

        context.RegisterSourceOutput(structDeclarations.Combine(interfaceDeclarations), (spc, pair) =>
        {
            var structs = pair.Left;
            var interfaces = pair.Right;

            var enumSources = GenerateEnumSources(structs,interfaces.First());
            foreach (var enumSource in enumSources)
            {
                spc.AddSource($"{enumSource.Key}_Enum.g.cs", SourceText.From(enumSource.Value, Encoding.UTF8));
            }

            foreach (var structDeclaration in structs)
            {
                foreach (var interfaceDeclaration in interfaces)
                {
                    var source = GeneratePartialStructSource(structDeclaration, interfaceDeclaration, structs);
                    spc.AddSource($"{structDeclaration.Identifier.Text}_Generated.g.cs", SourceText.From(source, Encoding.UTF8));
                }
            }
        });
    }

    private Dictionary<string, string> GenerateEnumSources(IEnumerable<StructDeclarationSyntax> structs,InterfaceDeclarationSyntax interfaceDeclarationSyntax)
    {
        var enumSources = new Dictionary<string, string>();
        var enumMembersByEnumName = new Dictionary<string, StringBuilder>();

        foreach (var structDeclaration in structs)
        {
            if (structDeclaration.AttributeLists.Any(a => a.Attributes.ToString() == "AbstractStruct"))
            {
                continue;
            }

            var headerField = GetHeaderField(interfaceDeclarationSyntax);
            if (!string.IsNullOrEmpty(headerField.EnumName))
            {
                if (!enumMembersByEnumName.ContainsKey(headerField.EnumName))
                {
                    enumMembersByEnumName[headerField.EnumName] = new StringBuilder();
                }

                var structName = structDeclaration.Identifier.Text;
                enumMembersByEnumName[headerField.EnumName].AppendLine($"{structName},");
            }
        }

        foreach (var enumName in enumMembersByEnumName.Keys)
        {
            var enumMembers = enumMembersByEnumName[enumName].ToString();
            var enumSource = $@"
namespace StructPolymorphismGenerator.Sample;

public enum {enumName}
{{
    {enumMembers}
}}
";
            enumSources[enumName] = enumSource;
        }

        return enumSources;
    }

    private string GeneratePartialStructSource(StructDeclarationSyntax structDeclaration, InterfaceDeclarationSyntax interfaceDeclaration, IEnumerable<StructDeclarationSyntax> allStructs)
    {
        var structName = structDeclaration.Identifier.Text;
        var interfaceName = interfaceDeclaration.Identifier.Text;

        var headerField = GetHeaderField(interfaceDeclaration);
        var headerElements = GetHeaderElements(interfaceDeclaration);

        var methods = new StringBuilder();
        foreach (var method in interfaceDeclaration.Members.OfType<MethodDeclarationSyntax>())
        {
            if (method.AttributeLists.Any(al => al.Attributes.Any(a => a.Name.ToString() == "StructVirtual")))
            {
                methods.AppendLine(GenerateVirtualMethod(method, headerField.Name, headerField.EnumName, allStructs));
            }
        }

        var properties = GenerateHeaderProperties(headerElements, headerField);

        return $@"
namespace StructPolymorphismGenerator.Sample;

public partial struct {structName} : {interfaceName}
{{
    public {headerField.Type} {headerField.Name} {{ get; }}

    {properties}

    {methods}
}}
";
    }

    private (string Type, string Name, string EnumName) GetHeaderField(InterfaceDeclarationSyntax structDeclaration)
    {
        var headerProperty = structDeclaration.Members.OfType<PropertyDeclarationSyntax>()
            .FirstOrDefault(p => p.AttributeLists.Any(al => al.Attributes.Any(a => a.Name.ToString() == "Header")));

        if (headerProperty != null)
        {
            var propertyType = headerProperty.Type.ToString();
            var propertyName = headerProperty.Identifier.Text;
            var enumName = headerProperty.AttributeLists
                .SelectMany(al => al.Attributes)
                .Where(a => a.Name.ToString() == "Header")
                .SelectMany(a => a.ArgumentList.Arguments)
                .FirstOrDefault()?.ToString().Trim('"');

            return (propertyType, propertyName, enumName);
        }

        return ("MissionHeader", "Header", "GeneratedEnum");
    }

    private IEnumerable<(string Type, string Name)> GetHeaderElements(InterfaceDeclarationSyntax interfaceDeclaration)
    {
        return interfaceDeclaration.Members.OfType<PropertyDeclarationSyntax>()
            .Where(p => p.AttributeLists.Any(al => al.Attributes.Any(a => a.Name.ToString() == "HeaderElement")))
            .Select(p => (p.Type.ToString(), p.Identifier.Text));
    }

    private string GenerateHeaderProperties(IEnumerable<(string Type, string Name)> headerElements, (string Type, string Name, string EnumName) headerField)
    {
        var properties = new StringBuilder();

        foreach (var element in headerElements)
        {
            properties.AppendLine($@"
public {element.Type} {element.Name} => {headerField.Name}.{element.Name};
");
        }

        return properties.ToString();
    }

    private string GenerateVirtualMethod(MethodDeclarationSyntax method, string headerFieldName, string enumName, IEnumerable<StructDeclarationSyntax> allStructs)
    {
        var methodName = method.Identifier.Text;
        var returnType = method.ReturnType.ToString();

        var cases = new StringBuilder();
        foreach (var structDeclaration in allStructs)
        {
            var structName = structDeclaration.Identifier.Text;
            cases.AppendLine($"{enumName}.{structName} => (({structName}*) mission)->{methodName}(),");
        }

        return $@"
public {returnType} {methodName}()
{{
    unsafe
    {{
        fixed (Mission* mission = &this)
        {{
            return {headerFieldName}.Type switch
            {{
                {cases}
                _ => false
            }};
        }}
    }}
}}
";
    }
}