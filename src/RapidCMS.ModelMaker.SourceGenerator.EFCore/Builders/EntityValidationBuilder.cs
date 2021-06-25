using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.Text;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Contexts;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Enums;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Information;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Builders
{
    internal sealed class EntityValidationBuilder : BuilderBase
    {
        public SourceText? BuildValidation(EntityInformation info, ModelMakerContext context)
        {
            if (!info.OutputItems.Contains(Constants.OutputValidation))
            {
                return default;
            }

            using var writer = new StringWriter();
            using var indentWriter = new IndentedTextWriter(writer, "    ");

            WriteUsingNamespaces(indentWriter, info.NamespacesUsed(Use.Validation));
            WriteOpenNamespace(indentWriter, context.Namespace);

            indentWriter.WriteLine($"public class {info.PascalCaseName}Validator : AbstractValidatorAdapter<{info.PascalCaseName}>");
            WriteOpeningBracket(indentWriter);

            indentWriter.WriteLine($"public {info.PascalCaseName}Validator()");
            WriteOpeningBracket(indentWriter);

            foreach (var property in info.Properties.Where(x => x.Validations.Any()))
            {
                WriteProperty(indentWriter, property, info, context);
            } 

            WriteClosingBracket(indentWriter);
            WriteClosingBracket(indentWriter);
            WriteClosingBracket(indentWriter);

            return SourceText.From(writer.ToString(), Encoding.UTF8);
        }

        private void WriteProperty(IndentedTextWriter indentWriter, PropertyInformation property, EntityInformation entity, ModelMakerContext context)
        {
            indentWriter.Write($"RuleFor(x => x.{property.PascalCaseName})");
            indentWriter.Indent++;

            foreach (var validation in property.Validations)
            {
                WriteValidation(indentWriter, validation, property, entity, context);
            }

            indentWriter.WriteLine(";");
            indentWriter.Indent--;
        }

        private void WriteValidation(IndentedTextWriter indentWriter, ValidationInformation validation, PropertyInformation property, EntityInformation entity, ModelMakerContext context)
        {
            indentWriter.WriteLine();
            indentWriter.Write($".{validation.ValidationMethodName}(");

            if (validation.Value is string stringValue)
            {
                indentWriter.Write($"\"{stringValue}\"");
            }
            else if (validation.Value is DateTime dateTimeValue)
            {
                indentWriter.Write($"new System.DateTime({dateTimeValue.Ticks})");
            }
            else if (validation.Value is object value)
            {
                indentWriter.Write(value.ToString());
            }
            else if (validation.Dictionary != null)
            {

            }

            indentWriter.Write(")");
        }
    }
}
