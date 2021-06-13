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
        public SourceText BuildEntityTypeConfiguration(EntityInformation info, ModelMakerContext context)
        {
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
            indentWriter.WriteLine($"public class {info.Name}Configuration : IEntityTypeConfiguration<{info.Name}>");
            WriteOpeningBracket(indentWriter);

            indentWriter.WriteLine($"public void Configure(EntityTypeBuilder<{info.Name}> builder)");
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
            indentWriter.Write("builder");

            if (property.RelatedToOneEntity)
            {
                indentWriter.Write($".HasOne(x => x.{property.Name})");
            }
            else
            {
                indentWriter.Write($".HasMany(x => x.{property.Name})");
            }

            indentWriter.Write($".WithMany(x => x.{entity.Name}{property.Name})");

            if (property.RelatedToOneEntity)
            {
                indentWriter.Write(".OnDelete(DeleteBehavior.NoAction)");
            }

            indentWriter.WriteLine(";");
        }
    }
}
