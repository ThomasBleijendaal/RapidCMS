using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Common.Data;

#nullable enable

namespace RapidCMS.Common.Models
{
    public class Collection
    {
        public Collection(string name, string alias, EntityVariant entityVariant, Type repositoryType)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Alias = alias ?? throw new ArgumentNullException(nameof(alias));
            EntityVariant = entityVariant ?? throw new ArgumentNullException(nameof(entityVariant));
            RepositoryType = repositoryType ?? throw new ArgumentNullException(nameof(repositoryType));
        }

        internal string Name { get; private set; }
        internal string Alias { get; private set; }

        public List<Collection> Collections { get; set; } = new List<Collection>();

        internal List<EntityVariant>? SubEntityVariants { get; set; }
        internal EntityVariant EntityVariant { get; private set; }

        internal EntityVariant GetEntityVariant(string? alias)
        {
            if (string.IsNullOrWhiteSpace(alias) || SubEntityVariants == null)
            {
                return EntityVariant;
            }
            else
            {
                return SubEntityVariants.First(x => x.Alias == alias);
            }
        }
        internal EntityVariant GetEntityVariant(IEntity entity)
        {
            return SubEntityVariants?.FirstOrDefault(x => x.Type == entity.GetType())
                ?? EntityVariant;
        }

        internal Type RepositoryType { get; private set; }
        internal IRepository Repository { get; set; }

        internal TreeView? TreeView { get; set; }

        internal ListView? ListView { get; set; }
        internal ListEditor? ListEditor { get; set; }

        internal Node? NodeView { get; set; }
        internal Node? NodeEditor { get; set; }
    }
}
