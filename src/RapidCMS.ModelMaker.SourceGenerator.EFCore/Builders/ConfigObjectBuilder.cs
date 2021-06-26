using System;
using System.CodeDom.Compiler;
using System.Linq;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Information;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Builders
{
    internal sealed class ConfigObjectBuilder : BuilderBase
    {
        public void WriteConfigObject(IndentedTextWriter indentWriter, PropertyDetailInformation detail)
        {
            if (detail.ConfigList != null || detail.ConfigProperties != null)
            {
                indentWriter.Write($"new {detail.ConfigType} {{ ");

                if (detail.ConfigList is not null)
                {
                    indentWriter.Write($"{detail.PropertyName} = new List<string> {{ ");
                    indentWriter.Write(string.Join(", ", detail.ConfigList.Select(x => $"\"{x}\"")));
                    indentWriter.Write(" }");
                }
                else if (detail.ConfigProperties is not null)
                {
                    indentWriter.Write($"{detail.PropertyName} = new {detail.ConfigSubType} {{ ");
                    indentWriter.Write(string.Join(", ", detail.ConfigProperties.Select(x => $"{x.Key} = {GeneratePrimitive(x.Value)}")));
                    indentWriter.Write(" }");
                }

                indentWriter.Write(" }");
            }
            else
            {
                indentWriter.Write(GeneratePrimitive(detail.Value));
            }
        }

        private static string GeneratePrimitive(object? value)
            => value switch
            {
                string stringValue => $"\"{stringValue}\"",
                DateTime dateTimeValue => $"new System.DateTime({dateTimeValue.Ticks})",
                object @object => @object.ToString(),
                _ => ""
            };
    }
}
