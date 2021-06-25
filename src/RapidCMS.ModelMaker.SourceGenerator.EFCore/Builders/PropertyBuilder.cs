using System.CodeDom.Compiler;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Enums;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Information;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Builders
{
    internal sealed class PropertyBuilder : BuilderBase
    {
        public void WriteProperty(IndentedTextWriter indentWriter, PropertyInformation info)
        {
            indentWriter.WriteLine();

            if (info.Relation.HasFlag(Relation.One | Relation.ToOne) && !info.Relation.HasFlag(Relation.DependentSide))
            {
                indentWriter.WriteLine($"public {info.Type}? {info.PascalCaseName} {{ get; set; }}");
            }
            else if (info.Relation.HasFlag(Relation.ToOne))
            {
                indentWriter.WriteLine($"public int? {info.PascalCaseName}Id {{ get; set; }}"); // TODO: how to detect type of ForeignKey type?
                indentWriter.WriteLine($"public {info.Type}? {info.PascalCaseName} {{ get; set; }}");
            }
            else if (info.Relation.HasFlag(Relation.ToMany))
            {
                indentWriter.WriteLine($"public ICollection<{info.Type}> {info.PascalCaseName} {{ get; set; }} = new List<{info.Type}>();");
            }
            else
            {
                indentWriter.WriteLine($"public {info.Type} {info.PascalCaseName} {{ get; set; }}");
            }
        }
    }
}
