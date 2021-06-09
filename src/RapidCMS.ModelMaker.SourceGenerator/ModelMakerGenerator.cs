using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using RapidCMS.ModelMaker.SourceGenerator.Abstractions;

namespace RapidCMS.ModelMaker.SourceGenerator
{
    [Generator]
    public class ModelMakerGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            
        }

        public void Execute(GeneratorExecutionContext context)
        {
            //foreach (var file in context.AdditionalFiles)
            //{
            //    context.ReportDiagnostic(Diagnostic.Create("RC0001", "", "fdsa", DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 1));
            //}

            context.AddSource("test.cs", "namespace Test { public class Test { } }");

            // generate sources
        }
    }
}
