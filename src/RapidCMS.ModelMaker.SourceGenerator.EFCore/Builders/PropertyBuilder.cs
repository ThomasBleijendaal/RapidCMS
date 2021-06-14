using System.CodeDom.Compiler;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Information;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Builders
{
    internal sealed class PropertyBuilder : BuilderBase
    {
        public void WriteProperty(IndentedTextWriter indentWriter, PropertyInformation info)
        {
            indentWriter.WriteLine();

            var name = ValidPascalCaseName(info.Name);

            foreach (var attribute in info.ValidationAttributes)
            {
                indentWriter.WriteLine(attribute);
            }

            if (info.RelatedToOneEntity)
            {
                indentWriter.WriteLine($"public int? {name}Id {{ get; set; }}"); // TODO: how to detect type of ForeignKey type?
                indentWriter.WriteLine($"public {info.Type}? {name} {{ get; set; }}");
            }
            else if (info.RelatedToManyEntities)
            {
                indentWriter.WriteLine($"public ICollection<{info.Type}> {name} {{ get; set; }} = new List<{info.Type}>();");
            }
            else
            {
                indentWriter.WriteLine($"public {info.Type} {name} {{ get; set; }}");
            }
        }
    }
}
