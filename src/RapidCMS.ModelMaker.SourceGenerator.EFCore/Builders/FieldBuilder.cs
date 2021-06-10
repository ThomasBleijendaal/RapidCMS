using System.CodeDom.Compiler;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Information;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Builders
{
    internal sealed class FieldBuilder : BuilderBase
    {
        public void WriteField(IndentedTextWriter indentWriter, PropertyInformation info)
        {
            indentWriter.Write($"section.AddField(x => x.{info.Name})");

            indentWriter.Write($".SetType(typeof({info.EditorType}))");

            indentWriter.WriteLine(";");
        }
    }
}
