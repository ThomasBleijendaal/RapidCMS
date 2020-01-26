using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Abstractions.Setup
{
    internal interface ICollectionSetup
    {
        string? Icon { get; }
        string Name { get; }
        string Alias { get; }
        bool Recursive { get; }

        List<CollectionSetup> Collections { get; }

        List<EntityVariantSetup>? SubEntityVariants { get; }
        EntityVariantSetup EntityVariant { get; }

        List<IDataView>? DataViews { get; }
        Type? DataViewBuilder { get; }

        EntityVariantSetup GetEntityVariant(string? alias);
        EntityVariantSetup GetEntityVariant(IEntity entity);

        Task<IEnumerable<IDataView>> GetDataViewsAsync(IServiceProvider serviceProvider);
        Task ProcessDataViewAsync(Query query, IServiceProvider serviceProvider);

        Type? RepositoryType { get; }

        TreeViewSetup? TreeView { get; }

        ListSetup? ListView { get; }
        ListSetup? ListEditor { get; }

        NodeSetup? NodeView { get; }
        NodeSetup? NodeEditor { get; }

        IButtonSetup? FindButton(string buttonId);
    }
}
