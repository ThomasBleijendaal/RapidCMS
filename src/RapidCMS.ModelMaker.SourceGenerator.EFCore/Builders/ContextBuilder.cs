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
    internal sealed class ContextBuilder : BuilderBase
    {
        public SourceText? BuildContext(ModelMakerContext context)
        {
            if (context.Entities.All(x => !x.OutputItems.Contains(Constants.OutputContext)))
            {
                return default;
            }

            using var writer = new StringWriter();
            using var indentWriter = new IndentedTextWriter(writer, "    ");

            var namespaces = context.Entities.SelectMany(x => x.NamespacesUsed(Use.Context));
            WriteUsingNamespaces(indentWriter, namespaces);
            WriteOpenNamespace(indentWriter, context.Namespace);

            WriteDbContext(indentWriter);
            WriteOnModelCreating(indentWriter, context);

            foreach (var entity in context.Entities.Where(x => x.OutputItems.Contains(Constants.OutputContext)))
            {
                WriteDbSet(indentWriter, entity);
            }

            WriteClosingBracket(indentWriter);

            WriteClosingBracket(indentWriter);

            return SourceText.From(writer.ToString(), Encoding.UTF8);
        }

        public void WriteDbContext(IndentedTextWriter indentWriter)
        {
            indentWriter.WriteLine($"public partial class ModelMakerDbContext : DbContext");
            WriteOpeningBracket(indentWriter);

            indentWriter.WriteLine($"public ModelMakerDbContext(DbContextOptions options) : base(options)");
            WriteOpeningBracket(indentWriter);
            WriteClosingBracket(indentWriter);
        }

        public void WriteOnModelCreating(IndentedTextWriter indentWriter, ModelMakerContext context)
        {
            indentWriter.WriteLine();
            indentWriter.WriteLine("protected override void OnModelCreating(ModelBuilder modelBuilder)");
            WriteOpeningBracket(indentWriter);

            indentWriter.WriteLine("base.OnModelCreating(modelBuilder);");
            indentWriter.WriteLine();
            
            foreach (var entity in context.Entities.Where(x => x.OutputItems.Contains(Constants.OutputContext)))
            {
                indentWriter.WriteLine($"modelBuilder.ApplyConfiguration(new {entity.Name}Configuration());");
            }

            WriteClosingBracket(indentWriter);
        }

        public void WriteDbSet(IndentedTextWriter indentWriter, EntityInformation info)
        {
            indentWriter.WriteLine();
            indentWriter.WriteLine($"public DbSet<{info.Name}> {info.PluralName} {{ get; set; }} = default!;");
        }
    }
}
