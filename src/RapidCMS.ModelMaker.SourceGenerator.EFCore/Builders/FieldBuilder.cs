using System.CodeDom.Compiler;
using System.Linq;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Enums;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Information;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Builders
{
    internal sealed class FieldBuilder : BuilderBase
    {
        private readonly ConfigObjectBuilder _configObjectBuilder;

        public FieldBuilder(ConfigObjectBuilder configObjectBuilder)
        {
            _configObjectBuilder = configObjectBuilder;
        }

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

            if (info.Details.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.DataCollectionType)) is PropertyDetailInformation details)
            {
                indentWriter.Write($".SetDataCollection<{details.DataCollectionType}");

                if (details.ConfigList != null || details.ConfigProperties != null)
                {

                    indentWriter.Write($", {details.ConfigType}>(");

                    _configObjectBuilder.WriteConfigObject(indentWriter, details);
                }
                else
                {
                    indentWriter.Write(">(");
                }

                indentWriter.Write(")");
            }
            else if (info.Relation.HasFlag(Relation.ToOne) && !string.IsNullOrEmpty(info.RelatedCollectionAlias))
            {
                indentWriter.Write($".SetCollectionRelation(\"{info.RelatedCollectionAlias}\")");
            }
            else if (info.Relation.HasFlag(Relation.ToMany) && !string.IsNullOrEmpty(info.RelatedCollectionAlias))
            {
                indentWriter.Write($".SetCollectionRelation(\"{info.RelatedCollectionAlias}\")");
            }

            if (info.IsSlugForTitleProperty() is string titleProperty)
            {
                indentWriter.Write($".SetConfiguration(new RapidCMS.Core.Models.UI.Configurability.DefaultValueEditorConfig<{info.Entity.Name}>(x => x.{titleProperty}?.ToUrlFriendlyString()))");
            }

            indentWriter.Write($".SetName(\"{info.Name}\")");

            indentWriter.WriteLine(";");
        }
    }
}
