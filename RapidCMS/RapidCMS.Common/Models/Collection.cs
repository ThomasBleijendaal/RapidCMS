using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RapidCMS.Common.ActionHandlers;
using RapidCMS.Common.Attributes;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;

#nullable enable

namespace RapidCMS.Common.Models
{
    // TODO: check polymorphisms
    // TODO: static root stuff is horrible

    // TODO: not really a model

        // TODO: everything should be internal
    public class Root : ICollectionRoot
    {
        public Root(
            IEnumerable<CustomButtonRegistration> customButtonRegistrations,
            IEnumerable<CustomEditorRegistration> customEditorRegistrations,
            IEnumerable<CustomSectionRegistration> customSectionRegistrations)
        {
            CustomButtonRegistrations = customButtonRegistrations.ToList();
            CustomEditorRegistrations = customEditorRegistrations.ToList();
            CustomSectionRegistrations = customSectionRegistrations.ToList();
        }

        private static Dictionary<string, Collection> _collectionMap { get; set; } = new Dictionary<string, Collection>();

        public List<CustomButtonRegistration> CustomButtonRegistrations { get; internal set; }
        public List<CustomEditorRegistration> CustomEditorRegistrations { get; internal set; }
        public List<CustomSectionRegistration> CustomSectionRegistrations { get; internal set; }

        public List<Collection> Collections { get; set; } = new List<Collection>();

        public string SiteName { get; set; } = "RapidCMS";

        internal void MaterializeRepositories(IServiceProvider serviceProvider)
        {
            FindRepositoryForCollections(serviceProvider, Collections);
        }

        internal Collection GetCollection(string alias)
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

    // TODO: check if these Registration classes can be merged
    public class CustomButtonRegistration
    {
        public CustomButtonRegistration(Type buttonType)
        {
            ButtonType = buttonType ?? throw new ArgumentNullException(nameof(buttonType));
            ButtonAlias = buttonType.FullName;
        }

        public Type ButtonType { get; set; }
        public string ButtonAlias { get; set; }
    }

    public class CustomEditorRegistration
    {
        public CustomEditorRegistration(Type editorType)
        {
            EditorType = editorType ?? throw new ArgumentNullException(nameof(editorType));
            EditorAlias = editorType.FullName;
        }

        public Type EditorType { get; set; }
        public string EditorAlias { get; set; }
    }

    public class CustomSectionRegistration
    {
        public CustomSectionRegistration(Type sectionType)
        {
            SectionType = sectionType ?? throw new ArgumentNullException(nameof(sectionType));
            SectionAlias = sectionType.FullName;
        }

        public Type SectionType { get; set; }
        public string SectionAlias { get; set; }
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
        public ViewPane? ViewPane { get; set; }
        public List<Button>? Buttons { get; set; }
    }

    public class Editor
    {
        public bool IsDefault { get; set; }
    }

    public class ListEditor : Editor
    {
        public ListEditorType ListEditorType { get; set; }
        public List<EditorPane> EditorPanes { get; set; }
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
        public List<EditorPane> EditorPanes { get; set; }
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

    public class EditorPane
    {
        public string? CustomAlias { get; set; }
        public string? Label { get; set; }
        public Type VariantType { get; set; }
        public List<Button> Buttons { get; set; }
        public List<Field> Fields { get; set; }
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
        public Task HandleActionAsync(string? parentId, string? id, object? customData)
        {
            return ActionHandler.InvokeAsync(parentId, id, customData);
        }
    }


    public class Field
    {
        public int Index { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public bool Readonly { get; set; } = true;

        public EditorType DataType { get; set; } = EditorType.Readonly;

        public IPropertyMetadata NodeProperty { get; set; }
        public Type ValueMapperType { get; set; }

        public OneToManyRelation? OneToManyRelation { get; set; }
    }

    public class CustomField : Field
    {
        public CustomField(Type customFieldType)
        {
            Alias = customFieldType?.FullName ?? throw new ArgumentNullException(nameof(customFieldType));
        }

        public string Alias { get; set; }
    }

    // TODO: horrible names
    public class OneToManyRelation
    {
    }

    public class OneToManyCollectionRelation : OneToManyRelation
    {
        internal string CollectionAlias { get; set; }
        internal IPropertyMetadata IdProperty { get; set; }
        internal IExpressionMetadata DisplayProperty { get; set; }
    }

    public class OneToManyDataProviderRelation : OneToManyRelation
    {
        internal Type DataProviderType { get; set; }
    }
}
