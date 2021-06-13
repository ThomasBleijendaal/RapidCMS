using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using Microsoft.CodeAnalysis.Text;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Enums;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Information;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Builders
{
    internal sealed class EntityBuilder : BuilderBase
    {
        private readonly string _namespace;
        private readonly PropertyBuilder _propertyBuilder;

        public EntityBuilder(string @namespace,
            PropertyBuilder propertyBuilder)
        {
            _namespace = @namespace;
            _propertyBuilder = propertyBuilder;
        }

        public SourceText BuildEntity(EntityInformation info)
        {
            using var writer = new StringWriter();
            using var indentWriter = new IndentedTextWriter(writer, "    ");

            WriteUsingNamespaces(indentWriter, info.NamespacesUsed(Use.Entity));
            WriteOpenNamespace(indentWriter, _namespace);

            WriteOpenEntity(indentWriter, info);

            WriteIdProperties(indentWriter);

            foreach (var property in info.Properties)
            {
                _propertyBuilder.WriteProperty(indentWriter, property);
            }

            WriteClosingBracket(indentWriter);
            WriteClosingBracket(indentWriter);

            return SourceText.From(writer.ToString(), Encoding.UTF8);
        }

        private void WriteOpenEntity(IndentedTextWriter indentWriter, EntityInformation info)
        {
            indentWriter.WriteLine($"public class {info.Name} : IEntity");
            indentWriter.WriteLine("{");
            indentWriter.Indent++;
        }

        private void WriteIdProperties(IndentedTextWriter indentWriter)
        {
            indentWriter.WriteLine("public int Id { get; set; }");
            indentWriter.WriteLine("string? IEntity.Id { get => Id.ToString(); set => Id = int.Parse(value ?? \"0\"); }");
        }
    }
}
