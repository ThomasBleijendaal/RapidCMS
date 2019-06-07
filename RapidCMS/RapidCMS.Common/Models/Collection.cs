using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Common.Data;

#nullable enable

namespace RapidCMS.Common.Models
{
    public class Collection
    {
        internal string Name { get; set; }
        internal string Alias { get; set; }

        public List<Collection> Collections { get; set; } = new List<Collection>();

        internal List<EntityVariant>? SubEntityVariants { get; set; }
        internal EntityVariant EntityVariant { get; set; }

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

        internal Type RepositoryType { get; set; }
        internal IRepository Repository { get; set; }

        internal TreeView? TreeView { get; set; }

        internal ListView ListView { get; set; }
        internal ListEditor ListEditor { get; set; }

        internal Node NodeView { get; set; }
        internal Node NodeEditor { get; set; }
    }
}
