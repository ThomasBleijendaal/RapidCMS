using System.Collections.Generic;
using System.Linq;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Abstractions;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Information
{
    internal sealed class EntityInformation : InformationBase, IInformation
    {
        private readonly List<PropertyInformation> _properties = new();

        public EntityInformation()
        {
            _namespaces.Add("RapidCMS.Core.Abstractions.Data");
        }

        public string? Name { get; private set; }

        public EntityInformation HasName(string name)
        {
            Name = name;
            return this;
        }

        public IReadOnlyList<PropertyInformation> Properties => _properties;

        public EntityInformation AddProperty(PropertyInformation property)
        {
            _properties.Add(property);
            return this;
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Name)
                && _properties.All(x => x.IsValid());
        }

        public IEnumerable<string> NamespacesUsed()
            => _namespaces.Union(_properties.SelectMany(x => x.NamespacesUsed()));
    }
}
