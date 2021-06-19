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

            foreach (var attribute in info.ValidationAttributes)
            {
                indentWriter.WriteLine(attribute);
            }

            if (info.Relation.HasFlag(Relation.ToOne))
            {
                indentWriter.WriteLine($"public int? {info.PascalName}Id {{ get; set; }}"); // TODO: how to detect type of ForeignKey type?
                indentWriter.WriteLine($"public {info.Type}? {info.PascalName} {{ get; set; }}");
            }
            else if (info.Relation.HasFlag(Relation.ToMany))
            {
                indentWriter.WriteLine($"public ICollection<{info.Type}> {info.PascalName} {{ get; set; }} = new List<{info.Type}>();");
            }
            else
            {
                indentWriter.WriteLine($"public {info.Type} {info.PascalName} {{ get; set; }}");
            }
        }
    }
}
