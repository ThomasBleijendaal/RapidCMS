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
    internal sealed class CollectionBuilder : BuilderBase
    {
        private readonly FieldBuilder _fieldBuilder;

        public CollectionBuilder(FieldBuilder fieldBuilder)
        {
            _fieldBuilder = fieldBuilder;
        }

        public SourceText BuildCollection(EntityInformation info, ModelMakerContext context)
        {
            using var writer = new StringWriter();
            using var indentWriter = new IndentedTextWriter(writer, "    ");

            var namespaces = info.NamespacesUsed(Use.Collection);
            WriteUsingNamespaces(indentWriter, namespaces);
            WriteOpenNamespace(indentWriter, context.Namespace);

            WriteOpenStaticClass(indentWriter, info);

            WriteTreeConfig(indentWriter, info);
            WriteElementConfig(indentWriter, info);

            WriteListView(indentWriter, info);
            WriteNodeEditor(indentWriter, info);

            WriteClosingLambda(indentWriter);
            indentWriter.Indent--;

            WriteClosingBracket(indentWriter);
            WriteClosingBracket(indentWriter);
            WriteClosingBracket(indentWriter);

            return SourceText.From(writer.ToString(), Encoding.UTF8);
        }

        public void WriteOpenStaticClass(IndentedTextWriter indentWriter, EntityInformation info)
        {
            indentWriter.WriteLine($"public static class {info.Name}Collection");
            indentWriter.WriteLine("{");
            indentWriter.Indent++;

            indentWriter.WriteLine($"public static void Add{info.Name}Collection(this ICmsConfig config)");
            indentWriter.WriteLine("{");
            indentWriter.Indent++;

            indentWriter.WriteLine($"config.AddCollection<{info.Name}, BaseRepository<{info.Name}>>(");
            indentWriter.Indent++;

            indentWriter.WriteLine($"\"{info.Alias}\",");
            indentWriter.WriteLine($"\"Database\","); // TODO: icon
            indentWriter.WriteLine($"\"Cyan30\","); // TODO: color
            indentWriter.WriteLine($"\"{info.Name}\",");
            indentWriter.WriteLine("collection =>");
            indentWriter.WriteLine("{");
            indentWriter.Indent++;
        }

        public void WriteTreeConfig(IndentedTextWriter indentWriter, EntityInformation info)
        {
            var titleProperty = info.Properties.Single(x => x.IsTitleOfEntity);

            indentWriter.WriteLine($"collection.SetTreeView(x => x.{ValidPascalCaseName(titleProperty.Name)});");
        }

        public void WriteElementConfig(IndentedTextWriter indentWriter, EntityInformation info)
        {
            var titleProperty = info.Properties.Single(x => x.IsTitleOfEntity);

            indentWriter.WriteLine($"collection.SetElementConfiguration(x => x.Id, x => x.{ValidPascalCaseName(titleProperty.Name)});");
        }

        public void WriteListView(IndentedTextWriter indentWriter, EntityInformation info)
        {
            var titleProperty = info.Properties.Single(x => x.IsTitleOfEntity);

            indentWriter.WriteLine("collection.SetListView(view =>");
            indentWriter.WriteLine("{");
            indentWriter.Indent++;

            indentWriter.WriteLine("view.AddDefaultButton(DefaultButtonType.New);");
            indentWriter.WriteLine("view.AddRow(row =>");
            indentWriter.WriteLine("{");
            indentWriter.Indent++;

            indentWriter.WriteLine("row.AddField(x => x.Id.ToString()).SetName(\"Id\");");
            indentWriter.WriteLine($"row.AddField(x => x.{ValidPascalCaseName(titleProperty.Name)});");
            indentWriter.WriteLine("row.AddDefaultButton(DefaultButtonType.Edit);");
            indentWriter.WriteLine("row.AddDefaultButton(DefaultButtonType.Delete);");

            WriteClosingLambda(indentWriter);
            WriteClosingLambda(indentWriter);
        }

        public void WriteNodeEditor(IndentedTextWriter indentWriter, EntityInformation info)
        {
            indentWriter.WriteLine("collection.SetNodeEditor(editor =>");
            indentWriter.WriteLine("{");
            indentWriter.Indent++;

            indentWriter.WriteLine("editor.AddDefaultButton(DefaultButtonType.Up);");
            indentWriter.WriteLine("editor.AddDefaultButton(DefaultButtonType.SaveExisting);");
            indentWriter.WriteLine("editor.AddDefaultButton(DefaultButtonType.SaveNew);");
            indentWriter.WriteLine("editor.AddSection(section =>");
            indentWriter.WriteLine("{");
            indentWriter.Indent++;

            foreach (var property in info.Properties.Where(x => x.Uses.HasFlag(Use.Collection)))
            {
                _fieldBuilder.WriteField(indentWriter, property);
            }

            WriteClosingLambda(indentWriter);
            WriteClosingLambda(indentWriter);
        }
    }
}
