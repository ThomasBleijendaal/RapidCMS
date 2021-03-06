﻿using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.Setup
{
    internal class TreeViewSetup : ITreeViewSetup
    {
        public TreeViewSetup(EntityVisibilty entityVisibility, CollectionRootVisibility rootVisibility, bool defaultOpenEntities, bool defaultOpenCollections, IExpressionMetadata? name)
        {
            EntityVisibility = entityVisibility;
            RootVisibility = rootVisibility;
            DefaultOpenEntities = defaultOpenEntities;
            DefaultOpenCollections = defaultOpenCollections;
            Name = name;
        }

        public EntityVisibilty EntityVisibility { get; set; }
        public CollectionRootVisibility RootVisibility { get; set; }
        public bool DefaultOpenEntities { get; set; }
        public bool DefaultOpenCollections { get; set; }

        public IExpressionMetadata? Name { get; set; }
    }
}
