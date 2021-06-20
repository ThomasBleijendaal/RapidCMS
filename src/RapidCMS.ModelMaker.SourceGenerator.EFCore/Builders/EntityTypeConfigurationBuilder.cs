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
            indentWriter.WriteLine($"public class {info.PascalCaseName}Configuration : IEntityTypeConfiguration<{info.PascalCaseName}>");
            WriteOpeningBracket(indentWriter);

            indentWriter.WriteLine($"public void Configure(EntityTypeBuilder<{info.PascalCaseName}> builder)");
            WriteOpeningBracket(indentWriter);

            foreach (var property in info.Properties.Where(x => !x.Hidden && x.Relation != Relation.None))
            {
                WriteRelationConfig(indentWriter, info, property);
            }

            WriteClosingBracket(indentWriter);
            WriteClosingBracket(indentWriter);
        }

        public void WriteRelationConfig(IndentedTextWriter indentWriter, EntityInformation entity, PropertyInformation property)
        {
            if (property.Relation.HasFlag(Relation.One | Relation.ToOne | Relation.DependentSide))
            {
                return;
            }

            indentWriter.Write("builder");
            
            if (property.Relation.HasFlag(Relation.ToOne))
            {
                indentWriter.Write($".HasOne(x => x.{property.PascalCaseName})");
            }
            else
            {
                indentWriter.Write($".HasMany(x => x.{property.PascalCaseName})");
            }

            if (property.Relation.HasFlag(Relation.One))
            {
                indentWriter.Write($".WithOne(x => x.{property.RelatedPropertyName})");
            }
            else
            {
                indentWriter.Write($".WithMany(x => x.{property.RelatedPropertyName})");
            }

            if (property.Relation.HasFlag(Relation.One | Relation.ToOne))
            {
                indentWriter.Write($".HasForeignKey<{property.Type}>(x => x.{property.RelatedPropertyName}Id)");
            }

            if (!property.Relation.HasFlag(Relation.Many | Relation.ToMany))
            {
                indentWriter.Write(".OnDelete(DeleteBehavior.NoAction)");
            }

            indentWriter.WriteLine(";");
        }
    }
}
