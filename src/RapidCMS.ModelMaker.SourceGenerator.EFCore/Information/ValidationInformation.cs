using System.Collections.Generic;
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

        public ValidationInformation(string validationConfigType, string? validationNamespace, string validationMethodName)
        {
            ValidationConfigType = validationConfigType ?? throw new System.ArgumentNullException(nameof(validationConfigType));
            ValidationNamespace = validationNamespace;
            ValidationMethodName = validationMethodName ?? throw new System.ArgumentNullException(nameof(validationMethodName));
        }

        public bool IsValid()
        {
            return true;
        }

        public IEnumerable<string> NamespacesUsed(Use use)
        {
            if (use != Use.Validation)
            {
                yield break;
            }

            if (!string.IsNullOrWhiteSpace(ValidationNamespace))
            {
                yield return ValidationNamespace!;
            }

            if (List != null)
            {
                yield return "System.Collections.Generic";
            }
        }

        public string? ValidationConfigType { get; private set; }
        public string? ValidationNamespace { get; private set; }
        public string ValidationMethodName { get; private set; }

        public object? Value { get; set; }

        public ValidationInformation HasValue(object value)
        {
            Value = value;

            return this;
        }

        public string? PropertyName { get; set; }

        public List<string>? List { get; set; }

        public ValidationInformation HasList(string propertyName, List<string> list)
        {
            PropertyName = propertyName;
            List = list;

            return this;
        }

        public string? ConfigObjectName { get; private set; }
        
        public IReadOnlyDictionary<string, string>? Dictionary { get; private set; }

        public ValidationInformation HasDictionary(Dictionary<string, string> dict)
        {
            Dictionary = dict;

            return this;
        }
    }
}
