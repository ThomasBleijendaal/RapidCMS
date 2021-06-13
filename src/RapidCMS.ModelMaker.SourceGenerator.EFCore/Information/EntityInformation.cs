using System.Collections.Generic;
using System.Linq;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Abstractions;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Enums;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Information
{
    internal sealed class EntityInformation : InformationBase, IInformation
    {
        private readonly List<PropertyInformation> _properties = new();

        public EntityInformation()
        {
            _namespaces.Add((Use.Entity, "RapidCMS.Core.Abstractions.Data"));
            _namespaces.Add((Use.Collection, "RapidCMS.Core.Abstractions.Config"));
            _namespaces.Add((Use.Collection, "RapidCMS.Core.Enums"));
            _namespaces.Add((Use.Collection, "RapidCMS.Core.Providers"));
            _namespaces.Add((Use.Collection, "RapidCMS.Core.Repositories"));
            _namespaces.Add((Use.Context, "Microsoft.EntityFrameworkCore"));
        }

        public string? Name { get; private set; }

        public EntityInformation HasName(string name)
        {
            Name = name;
            return this;
        }

        public string? Alias { get; private set; }

        public EntityInformation HasAlias(string alias)
        {
            Alias = alias;
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
            return !string.IsNullOrEmpty(Name) &&
                !string.IsNullOrEmpty(Alias) &&
                _properties.Count(x => x.IsTitleOfEntity) == 1 &&
                _properties.All(x => x.IsValid());
        }

        public IEnumerable<string> NamespacesUsed(Use use)
            => _namespaces
                .Where(x => x.use.HasFlag(use))
                .Select(x => x.@namespace)
                .Union(_properties.SelectMany(x => x.NamespacesUsed(use)));
    }
}
