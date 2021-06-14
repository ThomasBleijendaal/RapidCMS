using System;
using System.Collections.Generic;
using System.Text;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Information;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Contexts
{
    internal class ModelMakerContext
    {
        private readonly IReadOnlyList<EntityInformation> _entities;

        public ModelMakerContext(string @namespace, IReadOnlyList<EntityInformation> entities)
        {
            Namespace = @namespace;
            _entities = entities ?? throw new ArgumentNullException(nameof(entities));
        }

        public IEnumerable<EntityInformation> Entities => _entities;

        public string Namespace { get; }
    }
}
