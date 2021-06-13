using System.CodeDom.Compiler;
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

            if (info.RelatedToOneEntity)
            {
                indentWriter.WriteLine($"public int? {info.Name}Id {{ get; set; }}"); // TODO: how to detect type of ForeignKey type?
                indentWriter.WriteLine($"public {info.Type}? {info.Name} {{ get; set; }}");
            }
            else if (info.RelatedToManyEntities)
            {
                indentWriter.WriteLine($"public ICollection<{info.Type}> {info.Name} {{ get; set; }} = new List<{info.Type}>();");
            }
            else
            {
                indentWriter.WriteLine($"public {info.Type} {info.Name} {{ get; set; }}");
            }
        }
    }
}
