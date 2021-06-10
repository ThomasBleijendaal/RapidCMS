using System.Collections.Generic;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Abstractions;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Information
{
    internal sealed class PropertyInformation : InformationBase, IInformation
    {
        public PropertyInformation()
        {
        }

        public string? Name { get; private set; }

        public PropertyInformation HasName(string name)
        {
            Name = name;
            return this;
        }

        public string? Type { get; private set; }

        public PropertyInformation IsType(string type)
        {
            Type = type;
            return this;
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Type);
        }

        public IEnumerable<string> NamespacesUsed()
            => _namespaces;
    }
}
