﻿using System;
using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Abstractions.Setup
{
    internal interface ICollectionSetup
    {
        string? Icon { get; }
        string? Color { get; }
        string Alias { get; }
        string RepositoryAlias { get; }
        string Name { get; }
        bool Recursive { get; }

        ITreeElementSetup? Parent { get; }
        List<ITreeElementSetup> Collections { get; }

        List<IEntityVariantSetup>? SubEntityVariants { get; }
        IEntityVariantSetup EntityVariant { get; }

        List<IDataView>? DataViews { get; }
        Type? DataViewBuilder { get; }

        IEntityVariantSetup GetEntityVariant(string? alias);
        IEntityVariantSetup GetEntityVariant(IEntity entity);

        TreeViewSetup? TreeView { get; }

        ListSetup? ListView { get; }
        ListSetup? ListEditor { get; }

        NodeSetup? NodeView { get; }
        NodeSetup? NodeEditor { get; }

        IButtonSetup? FindButton(string buttonId);
    }
}
