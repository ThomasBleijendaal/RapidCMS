using System.CodeDom.Compiler;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Information;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Builders
{
    internal sealed class FieldBuilder : BuilderBase
    {
        public void WriteField(IndentedTextWriter indentWriter, PropertyInformation info)
        {
            if (info.RelatedToOneEntity)
            {
                indentWriter.Write($"section.AddField(x => x.{info.Name}Id)");
            }
            else
            {
                indentWriter.Write($"section.AddField(x => x.{info.Name})");
            }

            indentWriter.Write($".SetType(typeof({info.EditorType}))");

            if (!string.IsNullOrEmpty(info.DataCollectionExpression))
            {
                indentWriter.Write($".SetDataCollection({info.DataCollectionExpression})");
            }
            else if (info.RelatedToOneEntity && !string.IsNullOrEmpty(info.RelatedCollectionAlias))
            {
                indentWriter.Write($".SetCollectionRelation(\"{info.RelatedCollectionAlias}\")");
            }
            else if (info.RelatedToManyEntities && !string.IsNullOrEmpty(info.RelatedCollectionAlias))
            {
                indentWriter.Write($".SetCollectionRelation(\"{info.RelatedCollectionAlias}\")");
            }

            indentWriter.WriteLine(";");
        }
    }
}
