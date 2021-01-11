﻿using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.ApiBridge.Request
{
    public class RelateModel
    {
        public RelateModel() { }

        public RelateModel(IRelated related, string id)
        {
            Id = id;
            Related = new EntityDescriptorModel
            {
                RepositoryAlias = related.RepositoryAlias,
                Id = related.Entity.Id!,
                ParentPath = related.Parent?.GetParentPath()?.ToPathString()
            };
        }

        public string Id { get; set; } = default!;

        public EntityDescriptorModel Related { get; set; } = default!;
    }
}
