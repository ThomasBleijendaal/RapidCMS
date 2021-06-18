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
    internal sealed class EntityTypeConfigurationBuilder : BuilderBase
    {
        public SourceText? BuildEntityTypeConfiguration(EntityInformation info, ModelMakerContext context)
        {
            if (!info.OutputItems.Contains(Constants.OutputContext))
            {
                return default;
            }

            using var writer = new StringWriter();
            using var indentWriter = new IndentedTextWriter(writer, "    ");

            var namespaces = context.Entities.SelectMany(x => x.NamespacesUsed(Use.Context));
            WriteUsingNamespaces(indentWriter, namespaces);
            WriteOpenNamespace(indentWriter, context.Namespace);

            WriteEntityTypeConfiguration(indentWriter, info);

            WriteClosingBracket(indentWriter);

            return SourceText.From(writer.ToString(), Encoding.UTF8);
        }

        public void WriteEntityTypeConfiguration(IndentedTextWriter indentWriter, EntityInformation info)
        {
            var name = ValidPascalCaseName(info.Name);

            indentWriter.WriteLine($"public class {name}Configuration : IEntityTypeConfiguration<{name}>");
            WriteOpeningBracket(indentWriter);

            indentWriter.WriteLine($"public void Configure(EntityTypeBuilder<{name}> builder)");
            WriteOpeningBracket(indentWriter);

            foreach (var property in info.Properties.Where(x => !x.Hidden && (x.RelatedToOneEntity || x.RelatedToManyEntities)))
            {
                WriteRelationConfig(indentWriter, info, property);
            }

            WriteClosingBracket(indentWriter);
            WriteClosingBracket(indentWriter);
        }

        public void WriteRelationConfig(IndentedTextWriter indentWriter, EntityInformation entity, PropertyInformation property)
        {
            var propertyName = ValidPascalCaseName(property.Name);
            var entityName = ValidPascalCaseName(entity.Name);

            indentWriter.Write("builder");

            if (property.RelatedToOneEntity)
            {
                indentWriter.Write($".HasOne(x => x.{propertyName})");
            }
            else
            {
                indentWriter.Write($".HasMany(x => x.{propertyName})");
            }

            indentWriter.Write($".WithMany(x => x.{entityName}{propertyName})");

            if (property.RelatedToOneEntity)
            {
                indentWriter.Write(".OnDelete(DeleteBehavior.NoAction)");
            }

            indentWriter.WriteLine(";");
        }
    }
}
