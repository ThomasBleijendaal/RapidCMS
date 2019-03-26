using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RapidCMS.Common.Attributes;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

#nullable enable

namespace RapidCMS.Common.Models
{
    // TODO: check polymorphisms
    // TODO: static root stuff is horrible

        // TODO: not really a model
    public class Root : ICollectionRoot
    {
        private static Dictionary<string, Collection> _collectionMap { get; set; } = new Dictionary<string, Collection>();

        public List<Collection> Collections { get; set; } = new List<Collection>();

        public void MaterializeRepositories(IServiceProvider serviceProvider)
        {
            FindRepositoryForCollections(serviceProvider, Collections);
        }

        public Collection GetCollection(string alias)
        {
            return _collectionMap[alias];
        }

        private void FindRepositoryForCollections(IServiceProvider serviceProvider, IEnumerable<Collection> collections)
        {
            foreach (var collection in collections)
            {
                // register each collection in flat dictionary
                _collectionMap.Add(collection.Alias, collection);

                var repo = serviceProvider.GetRequiredService(collection.RepositoryType);

                collection.Repository = (IRepository)repo;

                FindRepositoryForCollections(serviceProvider, collection.Collections);
            }
        }

        public static IRepository? GetRepository(string collectionAlias)
        {
            return _collectionMap.TryGetValue(collectionAlias, out var collection) ? collection.Repository : default;
        }
    }

    public class Collection
    {
        public string Name { get; set; }
        public string Alias { get; set; }

        public List<Collection> Collections { get; set; } = new List<Collection>();
        public List<EntityVariant> EntityVariants { get; set; }

        public Type RepositoryType { get; set; }
        public IRepository Repository { get; set; }

        public TreeView TreeView { get; set; }

        public ListView ListView { get; set; }
        public ListEditor ListEditor { get; set; }

        public NodeView NodeView { get; set; }
        public NodeEditor NodeEditor { get; set; }

    }

    public interface ICollectionRoot
    {
        List<Collection> Collections { get; set; }
    }

    public class View
    {
    }

    public enum ViewType
    {
        List,
        Tree
    }

    public class TreeView : View
    {
        public ViewType EntityViewType { get; set; }

        public Func<object, object> NameGetter { get; set; }

        public List<Button> Buttons { get; set; }
        public Button AddButton { get; set; }
        public Button RemoveButton { get; set; }
    }

    public class ListView : View
    {
        public List<ViewPane<ListViewProperty>> ViewPanes { get; set; }
        public List<Button> Buttons { get; set; }
    }

    public class Editor
    {
        public bool IsDefault { get; set; }
    }

    public class ListEditor : Editor
    {
        public ListEditorType ListEditorType { get; set; }
        public List<EditorPane<Field>> EditorPanes { get; set; }
        public List<Button> Buttons { get; set; }
    }

    public class SubCollectionListEditor : ListEditor
    {
        public int Index { get; set; }

        public string CollectionAlias { get; set; }
    }

    public class NodeView : View
    {
        public List<ViewPane<NodeViewProperty>> ViewPanes { get; set; }
        public List<Button> Buttons { get; set; }
    }

    public class NodeEditor : Editor
    {
        public Type BaseType { get; set; }
        public List<EditorPane<Field>> EditorPanes { get; set; }
        public List<Button> Buttons { get; set; }
    }

    public class EntityVariant
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public Type Type { get; set; }
        public string Alias { get; set; }
    }

    public class ViewPane<T>
        where T : Property
    {
        public string Name { get; set; }

        public List<T> Properties { get; set; }
        public List<Button> Buttons { get; set; }
    }

    public class EditorPane<T>
        where T : Field
    {
        public Type VariantType { get; set; }
        public List<Button> Buttons { get; set; }
        public List<T> Fields { get; set; }
        public List<SubCollectionListEditor> SubCollectionListEditors { get; set; }
    }

    // TODO: merge with field
    public class Property
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public PropertyMetadata NodeProperty { get; set; }
        public IValueMapper ValueMapper { get; set; }
        public Type ValueMapperType { get; set; }
    }

    // TODO: required?
    public class ListViewProperty : Property
    {

    }

    public class NodeViewProperty : Property
    {

    }

    public class EntityListViewProperty : Property
    {

    }

    public abstract class Button
    {
        public string ButtonId { get; set; }

        public string Label { get; set; }
        public string Icon { get; set; }
        public bool ShouldConfirm { get; set; }

        public List<Button> Buttons { get; set; }

        // TODO: how does this behave in custom buttons?
        public object Metadata { get; set; }

        public abstract CrudType GetCrudType();
        public virtual bool IsCompatibleWithView(ViewContext viewContext) { return true; }
    }

    public class DefaultButton : Button
    {
        public DefaultButtonType DefaultButtonType { get; set; }

        public override CrudType GetCrudType()
        {
            switch (DefaultButtonType)
            {
                case DefaultButtonType.New:
                    return CrudType.Create;
                case DefaultButtonType.SaveNew:
                    return CrudType.Insert;
                case DefaultButtonType.SaveExisting:
                    return CrudType.Update;
                case DefaultButtonType.Delete:
                    return CrudType.Delete;
                case DefaultButtonType.Edit:
                    return CrudType.Read;
                case DefaultButtonType.View:
                    return CrudType.View;
                default:
                    return 0;
            }
        }
        public override bool IsCompatibleWithView(ViewContext viewContext)
        {
            return DefaultButtonType.GetCustomAttribute<ActionsAttribute>().Usages?.Any(x => viewContext.Usage.HasFlag(x)) ?? false;
        }
    }

    public class CustomButton : Button
    {
        public string Alias { get; set; }

        public IButtonActionHandler ActionHandler { get; set; }

        public override CrudType GetCrudType()
        {
            return ActionHandler.GetCrudType();
        }
        public override bool IsCompatibleWithView(ViewContext viewContext)
        {
            return ActionHandler.IsCompatibleWithView(viewContext);
        }
        public Task HandleActionAsync()
        {
            return ActionHandler.InvokeAsync();
        }
    }


    // TODO: merge with property
    public class Field
    {
        public int Index { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public bool Readonly { get; set; }

        public EditorType DataType { get; set; }

        public PropertyMetadata NodeProperty { get; set; }
        public IValueMapper ValueMapper { get; set; }
        public Type ValueMapperType { get; set; }

        public OneToManyRelation? OneToManyRelation { get; set; }
    }

    // TODO: horrible names
    public class OneToManyRelation
    {
    }

    public class OneToManyCollectionRelation : OneToManyRelation
    {
        internal string CollectionAlias { get; set; }
        internal PropertyMetadata IdProperty { get; set; }
        internal PropertyMetadata DisplayProperty { get; set; }
    }

    public class OneToManyDataProviderRelation : OneToManyRelation
    {
        internal Type DataProviderType { get; set; }
    }
}
