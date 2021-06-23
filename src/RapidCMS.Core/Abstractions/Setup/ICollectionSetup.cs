using System;
using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Abstractions.Setup
{
    public interface ICollectionSetup
    {
        string? Icon { get; }
        string? Color { get; }
        string Alias { get; }
        string RepositoryAlias { get; }
        string Name { get; }

        UsageType UsageType { get; set; }

        ITreeElementSetup? Parent { get; }
        List<ITreeElementSetup> Collections { get; }

        List<IEntityVariantSetup>? SubEntityVariants { get; }
        IEntityVariantSetup EntityVariant { get; }

        List<IDataView>? DataViews { get; }
        Type? DataViewBuilder { get; }

        IEntityVariantSetup GetEntityVariant(string? alias);
        IEntityVariantSetup GetEntityVariant(IEntity entity);

        ITreeViewSetup? TreeView { get; }
        IElementSetup? ElementSetup { get; }

        IListSetup? ListView { get; }
        IListSetup? ListEditor { get; }

        INodeSetup? NodeView { get; }
        INodeSetup? NodeEditor { get; }

        IButtonSetup? FindButton(string buttonId);

        List<Type> Validators { get; }
    }
}
