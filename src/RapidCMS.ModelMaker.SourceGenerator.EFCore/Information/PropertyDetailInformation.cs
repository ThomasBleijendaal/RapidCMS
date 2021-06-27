using System.Collections.Generic;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Abstractions;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Enums;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Information
{
    internal sealed class PropertyDetailInformation : InformationBase, IInformation
    {
        public PropertyDetailInformation(string validationMethodName)
        {
            ValidationMethodName = validationMethodName ?? throw new System.ArgumentNullException(nameof(validationMethodName));
        }

        public PropertyDetailInformation(string configType, string @namespace)
        {
            ConfigType = configType ?? throw new System.ArgumentNullException(nameof(configType));
            Namespace = @namespace ?? throw new System.ArgumentNullException(nameof(@namespace));
        }

        public bool IsValid()
        {
            return true;
        }

        public IEnumerable<string> NamespacesUsed(Use use)
        {
            if ((use != Use.Validation && use != Use.Collection) ||
               (use == Use.Validation && string.IsNullOrWhiteSpace(ValidationMethodName)) ||
               (use == Use.Collection && ConfigList == null && ConfigProperties == null))
            {
                yield break;
            }

            if (!string.IsNullOrWhiteSpace(Namespace))
            {
                yield return Namespace!;
            }
            if (!string.IsNullOrWhiteSpace(ConfigSubNamespace))
            {
                yield return ConfigSubNamespace!;
            }

            if (ConfigList != null)
            {
                yield return "System.Collections.Generic";
            }
        }

        public string? ConfigType { get; private set; }
        public string? Namespace { get; private set; }

        public string? ValidationMethodName { get; private set; }

        public PropertyDetailInformation HasValidationMethod(string name)
        {
            ValidationMethodName = name;

            return this;
        }

        public string? DataCollectionType { get; set; }

        public PropertyDetailInformation HasDataCollection(string type)
        {
            DataCollectionType = type;

            return this;
        }

        public object? Value { get; set; }

        public PropertyDetailInformation HasConfigValue(object value)
        {
            Value = value;

            return this;
        }

        public string? PropertyName { get; set; }

        public List<string>? ConfigList { get; set; }

        public PropertyDetailInformation HasConfigList(string propertyName, List<string> list)
        {
            PropertyName = propertyName;
            ConfigList = list;

            return this;
        }

        public string? ConfigSubType { get; private set; }
        public string? ConfigSubNamespace { get; private set; }

        public IReadOnlyDictionary<string, object>? ConfigProperties { get; private set; }

        public PropertyDetailInformation HasSubClass(string propertyName, string type, string? @namespace, Dictionary<string, object> properties)
        {
            PropertyName = propertyName;
            ConfigSubType = type;
            ConfigSubNamespace = @namespace;
            ConfigProperties = properties;

            return this;
        }
    }
}
