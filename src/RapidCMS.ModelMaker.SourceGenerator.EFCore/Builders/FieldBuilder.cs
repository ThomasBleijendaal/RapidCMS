using System.CodeDom.Compiler;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Enums;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Information;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Builders
{
    internal sealed class FieldBuilder : BuilderBase
    {
        public void WriteField(IndentedTextWriter indentWriter, PropertyInformation info)
        {
            if (info.Relation.HasFlag(Relation.One | Relation.ToOne) && !info.Relation.HasFlag(Relation.DependentSide))
            {
                indentWriter.Write($"section.AddField(x => x.{info.PascalCaseName} == null ? 0 : x.{info.PascalCaseName}.Id)");
                indentWriter.Write(".DisableWhen((e, s) => true)");
            }
            else if (info.Relation.HasFlag(Relation.ToOne))
            {
                indentWriter.Write($"section.AddField(x => x.{info.PascalCaseName}Id)");
            }
            else
            {
                indentWriter.Write($"section.AddField(x => x.{info.PascalCaseName})");
            }

            indentWriter.Write($".SetType(typeof({info.EditorType}))");

            if (!string.IsNullOrEmpty(info.DataCollectionExpression))
            {
                indentWriter.Write($".SetDataCollection({info.DataCollectionExpression})");
            }
            else if (info.Relation.HasFlag(Relation.ToOne) && !string.IsNullOrEmpty(info.RelatedCollectionAlias))
            {
                indentWriter.Write($".SetCollectionRelation(\"{info.RelatedCollectionAlias}\")");
            }
            else if (info.Relation.HasFlag(Relation.ToMany) && !string.IsNullOrEmpty(info.RelatedCollectionAlias))
            {
                indentWriter.Write($".SetCollectionRelation(\"{info.RelatedCollectionAlias}\")");
            }

            indentWriter.Write($".SetName(\"{info.Name}\")");

            indentWriter.WriteLine(";");
        }
    }
}
