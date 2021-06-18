using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.Text;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Contexts;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Enums;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Information;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Builders
{
    internal sealed class EntityBuilder : BuilderBase
    {
        private readonly PropertyBuilder _propertyBuilder;

        public EntityBuilder(PropertyBuilder propertyBuilder)
        {
            _propertyBuilder = propertyBuilder;
        }

        public SourceText? BuildEntity(EntityInformation info, ModelMakerContext context)
        {
            if (!info.OutputItems.Contains(Constants.OutputEntity))
            {
                return default;
            }

            using var writer = new StringWriter();
            using var indentWriter = new IndentedTextWriter(writer, "    ");

            WriteUsingNamespaces(indentWriter, info.NamespacesUsed(Use.Entity));
            WriteOpenNamespace(indentWriter, context.Namespace);

            WriteOpenEntity(indentWriter, info);

            WriteIdProperties(indentWriter);

            foreach (var property in info.Properties.Where(x => x.Uses.HasFlag(Use.Entity)))
            {
                _propertyBuilder.WriteProperty(indentWriter, property);
            }

            WriteClosingBracket(indentWriter);
            WriteClosingBracket(indentWriter);

            return SourceText.From(writer.ToString(), Encoding.UTF8);
        }

        private void WriteOpenEntity(IndentedTextWriter indentWriter, EntityInformation info)
        {
            indentWriter.WriteLine($"public partial class {ValidPascalCaseName(info.Name)} : IEntity");
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
