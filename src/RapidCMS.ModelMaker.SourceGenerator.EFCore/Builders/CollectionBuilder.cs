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

        public SourceText? BuildCollection(EntityInformation info, ModelMakerContext context)
        {
            if (!info.OutputItems.Contains(Constants.OutputCollection))
            {
                return default;
            }

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
            indentWriter.WriteLine($"public static class {info.PascalName}Collection");
            indentWriter.WriteLine("{");
            indentWriter.Indent++;

            indentWriter.WriteLine($"public static void Add{info.PascalName}Collection(this ICmsConfig config)");
            indentWriter.WriteLine("{");
            indentWriter.Indent++;

            indentWriter.WriteLine($"config.AddCollection<{info.PascalName}, BaseRepository<{info.PascalName}>>(");
            indentWriter.Indent++;

            indentWriter.WriteLine($"\"{info.Alias}\",");
            indentWriter.WriteLine($"\"{info.Icon ?? "Database"}\",");
            indentWriter.WriteLine($"\"{info.IconColor ?? "Gray40"}\",");
            indentWriter.WriteLine($"\"{info.PluralName}\",");
            indentWriter.WriteLine("collection =>");
            indentWriter.WriteLine("{");
            indentWriter.Indent++;
        }

        public void WriteTreeConfig(IndentedTextWriter indentWriter, EntityInformation info)
        {
            var titleProperty = info.Properties.Single(x => x.IsTitleOfEntity);

            indentWriter.WriteLine($"collection.SetTreeView(x => x.{titleProperty.PascalName});");
        }

        public void WriteElementConfig(IndentedTextWriter indentWriter, EntityInformation info)
        {
            var titleProperty = info.Properties.Single(x => x.IsTitleOfEntity);

            indentWriter.WriteLine($"collection.SetElementConfiguration(x => x.Id, x => x.{titleProperty.PascalName});");
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
            indentWriter.WriteLine($"row.AddField(x => x.{titleProperty.PascalName}).SetName(\"{titleProperty.Name}\");");
            foreach (var listViewProperty in info.Properties.Where(x => x.IncludeInListView))
            {
                indentWriter.WriteLine($"row.AddField(x => x.{listViewProperty.PascalName} == null ? \"\" : x.{listViewProperty.PascalName}.ToString().Truncate(25)).SetName(\"{listViewProperty.Name}\");");
            }
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
