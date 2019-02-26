using System;
using RapidCMS.Common.Attributes;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Interfaces;

namespace RapidCMS.Common.Models.Config
{
    // TODO: this class is a bit wonky
    public class SubCollectionListEditorConfig
    {
        public string CollectionAlias { get; set; }
    }

    // HACK: this is a copy of ListEditorConfig<T>
    public class SubCollectionListEditorConfig<TEntity> : SubCollectionListEditorConfig
        where TEntity : IEntity
    {
        //public SubCollectionListEditorConfig<TEntity> AddDefaultButton(DefaultButtonType type, string label = null, string icon = null)
        //{
        //    var button = new DefaultButtonConfig
        //    {
        //        ButtonType = type,
        //        Icon = icon ?? type.GetCustomAttribute<DefaultIconLabelAttribute>().Icon,
        //        Label = label ?? type.GetCustomAttribute<DefaultIconLabelAttribute>().Label
        //    };

        //    Buttons.Add(button);

        //    return this;
        //}

        //public SubCollectionListEditorConfig<TEntity> SetEditor(Action<ListEditorPaneConfig<TEntity>> configure)
        //{
        //    var config = new ListEditorPaneConfig<TEntity>();

        //    configure.Invoke(config);

        //    ListEditors = config;

        //    return this;
        //}
    }
}
