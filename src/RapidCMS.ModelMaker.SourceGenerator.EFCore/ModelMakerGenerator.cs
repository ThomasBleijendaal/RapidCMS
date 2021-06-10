using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Newtonsoft.Json.Linq;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Builders;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Parsers;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore
{
    [Generator]
    public class ModelMakerGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {

        }

        public void Execute(GeneratorExecutionContext context)
        {
            var propertyParser = new PropertyParser();
            var entityParser = new EntityParser(propertyParser);

            var propertyBuilder = new PropertyBuilder();
            var entityBuilder = new EntityBuilder("RapidCMS.ModelMaker", propertyBuilder); // TODO: config?

            foreach (var file in context.AdditionalFiles)
            {
                if (file.GetText() is SourceText text)
                {
                    var jObject = JObject.Parse(text.ToString());

                    var entity = entityParser.ParseEntity(jObject);

                    if (entity.IsValid()) // TODO: unit test
                    {
                        var entitySourceText = entityBuilder.BuildEntity(entity);
                        context.AddSource($"ModelMaker_{entity.Name}.cs", entitySourceText);
                    }
                    // context.ReportDiagnostic(Diagnostic.Create("RC0001", "", json.Value<string>("$type"), DiagnosticSeverity.Warning, DiagnosticSeverity.Warning, true, 1));
                }
            }
        }
    }
}
