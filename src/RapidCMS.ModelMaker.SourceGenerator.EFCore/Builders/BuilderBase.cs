﻿using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Builders
{
    internal abstract class BuilderBase
    {
        public void WriteUsingNamespaces(IndentedTextWriter indentWriter, IEnumerable<string> usings)
        {
            var validUsings = usings
                .Distinct()
                .OrderBy(x => x.StartsWith("System") ? 1 : 2)
                .ThenBy(x => x.Replace(".", "").Replace(";", ""))
                .ToList();
            foreach (var usingStatement in validUsings)
            {
                indentWriter.WriteLine($"using {usingStatement};");
            }

            if (validUsings.Count > 0)
            {
                indentWriter.WriteLine();
            }
        }

        protected void WriteOpenNamespace(IndentedTextWriter indentWriter, string nameSpace)
        {
            indentWriter.WriteLine("#nullable enable");
            indentWriter.WriteLine();
            indentWriter.WriteLine($"namespace {nameSpace}");
            indentWriter.WriteLine("{");
            indentWriter.Indent++;
        }

        protected void WriteOpeningBracket(IndentedTextWriter indentWriter)
        {
            indentWriter.WriteLine("{");
            indentWriter.Indent++;
        }

        protected void WriteClosingBracket(IndentedTextWriter indentWriter)
        {
            indentWriter.Indent--;
            indentWriter.WriteLine("}");
        }

        protected void WriteClosingLambda(IndentedTextWriter indentWriter)
        {
            indentWriter.Indent--;
            indentWriter.WriteLine("});");
        }
    }
}
