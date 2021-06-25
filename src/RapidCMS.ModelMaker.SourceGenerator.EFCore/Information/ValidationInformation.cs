using System.Collections.Generic;
using System.Linq;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Abstractions;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Enums;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Information
{
    internal sealed class ValidationInformation : InformationBase, IInformation
    {
        public ValidationInformation(string validationMethodName)
        {
            ValidationMethodName = validationMethodName ?? throw new System.ArgumentNullException(nameof(validationMethodName));
        }

        public bool IsValid()
        {
            return true;
        }

        public IEnumerable<string> NamespacesUsed(Use use)
        {
            if (!string.IsNullOrWhiteSpace(ConfigNamespace))
            {
                yield return ConfigNamespace!;
            }
        }

        public string ValidationMethodName { get; private set; }

        public object? Value { get; set; }

        public ValidationInformation HasValue(object value)
        {
            Value = value;

            return this;
        }

        public string? ConfigObjectName { get; private set; }
        public string? ConfigNamespace { get; private set; }

        public IReadOnlyDictionary<string, string>? Dictionary { get; private set; }

        public ValidationInformation HasDictionary(Dictionary<string, string> dict)
        {
            Dictionary = dict;

            if (dict.TryGetValue("$type", out var fullType))
            {
                var type = fullType.Split(',')[0];

                var typeParts = type.Split('.');

                ConfigObjectName = typeParts.Last();

                if (typeParts.Length >= 2)
                {
                    ConfigNamespace = string.Join(".", typeParts.Take(typeParts.Length - 1));
                }
            }

            return this;
        }
    }
}
