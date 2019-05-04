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

        public List<EntityVariant>? SubEntityVariants { get; set; }
        public EntityVariant EntityVariant { get; set; }

        public EntityVariant GetEntityVariant(string? alias)
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
        public EntityVariant GetEntityVariant(IEntity entity)
        {
            return SubEntityVariants?.FirstOrDefault(x => x.Type == entity.GetType())
                ?? EntityVariant;
        }

        public Type RepositoryType { get; set; }
        public IRepository Repository { get; set; }

        public TreeView? TreeView { get; set; }

        public ListView ListView { get; set; }
        public ListEditor ListEditor { get; set; }

        public NodeEditor NodeEditor { get; set; }

    }

    public interface ICollectionRoot
    {
        List<Collection> Collections { get; set; }
    }

    public class View
    {
    }

    public enum EntityVisibilty
    {
        Visible,
        Hidden
    }

    public enum CollectionRootVisibility
    {
        Visible,
        Hidden
    }

    public class TreeView : View
    {
        public EntityVisibilty EntityVisibility { get; set; }
        public CollectionRootVisibility RootVisibility { get; set; }

        public Func<object, object> NameGetter { get; set; }

        public List<Button> Buttons { get; set; }
        public Button AddButton { get; set; }
        public Button RemoveButton { get; set; }
    }

    public class ListView : View
    {
        public ViewPane ViewPane { get; set; }
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

    public class ViewPane
    {
        public string Name { get; set; }

        public List<Field> Fields { get; set; }
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

    public abstract class Button
    {
        public string ButtonId { get; set; }

        public string Label { get; set; }
        public string Icon { get; set; }
        public bool ShouldConfirm { get; set; }
        public bool IsPrimary { get; set; }

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
        public Task HandleActionAsync(string? parentId, string? id)
        {
            return ActionHandler.InvokeAsync(parentId, id);
        }
    }


    public class Field
    {
        public int Index { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public bool Readonly { get; set; } = true;

        public EditorType DataType { get; set; } = EditorType.Readonly;

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
