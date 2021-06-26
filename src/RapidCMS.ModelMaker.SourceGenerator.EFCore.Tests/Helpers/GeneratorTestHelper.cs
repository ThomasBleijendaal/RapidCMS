using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests.Helpers
{

    public static class GeneratorTestHelper
    {
        private static (ImmutableArray<Diagnostic>, string[]) GetGeneratedOutput(string[] jsonFileNames)
        {
            var jsons = jsonFileNames.Select(File.ReadAllText);

            var references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
                .Select(_ => MetadataReference.CreateFromFile(_.Location))
                .Concat(new[] { MetadataReference.CreateFromFile(typeof(ModelMakerGenerator).Assembly.Location) });
            var compilation = CSharpCompilation.Create("generator", new SyntaxTree[] { },
                references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var generator = new ModelMakerGenerator();

            var driver = CSharpGeneratorDriver.Create(ImmutableArray.Create<ISourceGenerator>(generator), jsons.Select(x => new JsonText(x)));
            driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

            var trees = outputCompilation.SyntaxTrees.ToList();

            return (diagnostics, trees.Select(x => x.ToString()).ToArray());
        }

        public static void TestGeneratedCode(string[] sourceTexts, params string[] expectedOutputSourceTexts)
        {
            var (diagnostics, output) = GetGeneratedOutput(sourceTexts);

            Assert.AreEqual(0, diagnostics.Length, string.Join(", ", diagnostics.Select(x => x.GetMessage())));

            for (var i = 0; i < expectedOutputSourceTexts.Length; i++)
            {
                try
                {
                    Assert.AreEqual(expectedOutputSourceTexts[i], output.ElementAtOrDefault(i) ?? "", $"Error in file index: {i}");
                }
                catch
                {
                    throw;
                }
            }
        }

        public static void TestReportedDiagnostics(string[] sourceTexts, params string[] expectedDiagnosticErrors)
        {
            var (diagnostics, output) = GetGeneratedOutput(sourceTexts);

            var errorCodes = diagnostics.Select(x => x.Id).ToArray();

            Assert.AreEqual(expectedDiagnosticErrors.Length, diagnostics.Length, $"Found messages: {string.Join(", ", errorCodes)}.");

            foreach (var diagnostic in expectedDiagnosticErrors)
            {
                Assert.Contains(diagnostic, errorCodes);
            }
        }
    }
}
