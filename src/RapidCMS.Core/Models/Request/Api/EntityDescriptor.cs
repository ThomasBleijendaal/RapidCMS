using System;

namespace RapidCMS.Core.Models.Request.Api
{
    [Obsolete]
    public class EntityDescriptor
    {
        public EntityDescriptor(string? id, string? repositoryAlias, string? parentPath, string? variantAlias)
        {
            Id = id;
            RepositoryAlias = repositoryAlias;
            ParentPath = parentPath;
            VariantAlias = variantAlias;
        }

        public string? Id { get; private set; }
        public string? RepositoryAlias { get; private set; }
        public string? ParentPath { get; private set; }
        public string? VariantAlias { get; private set; }
    }
}
