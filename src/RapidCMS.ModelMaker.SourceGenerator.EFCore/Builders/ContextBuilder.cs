using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.Text;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Enums;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Information;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Builders
{
    internal sealed class ContextBuilder : BuilderBase
    {
        private readonly string _namespace;

        public ContextBuilder(string @namespace)
        {
            _namespace = @namespace;
        }

        public SourceText BuildContext(IEnumerable<EntityInformation> info)
        {
            using var writer = new StringWriter();
            using var indentWriter = new IndentedTextWriter(writer, "    ");

            var namespaces = info.SelectMany(x => x.NamespacesUsed(Use.Context));
            WriteUsingNamespaces(indentWriter, namespaces);
            WriteOpenNamespace(indentWriter, _namespace);

            WriteDbContext(indentWriter);

            foreach (var entity in info)
            {
                WriteDbSet(indentWriter, entity);
            }

            WriteClosingBracket(indentWriter);

            WriteClosingBracket(indentWriter);

            return SourceText.From(writer.ToString(), Encoding.UTF8);
        }

        public void WriteDbContext(IndentedTextWriter indentWriter)
        {
            indentWriter.WriteLine($"public class ModelMakerDbContext : DbContext");
            indentWriter.WriteLine("{");
            indentWriter.Indent++;

            indentWriter.WriteLine($"public ModelMakerDbContext(DbContextOptions options) : base(options)");
            indentWriter.WriteLine("{");
            indentWriter.Indent++;
            WriteClosingBracket(indentWriter);
        }

        public void WriteDbSet(IndentedTextWriter indentWriter, EntityInformation info)
        {
            indentWriter.WriteLine();
            indentWriter.WriteLine($"public DbSet<{info.Name}> {info.Name} {{ get; set; }} = default!;");
        }
    }
}
