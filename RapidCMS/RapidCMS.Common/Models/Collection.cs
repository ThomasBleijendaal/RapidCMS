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
    // TODO: static root stuff is horrible

    // TODO: not really a model
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
        internal CustomButtonRegistration(Type buttonType)
        {
            ButtonType = buttonType ?? throw new ArgumentNullException(nameof(buttonType));
            ButtonAlias = buttonType.FullName;
        }

        public Type ButtonType { get; set; }
        public string ButtonAlias { get; set; }
    }

    public class CustomEditorRegistration
    {
        internal CustomEditorRegistration(Type editorType)
        {
            EditorType = editorType ?? throw new ArgumentNullException(nameof(editorType));
            EditorAlias = editorType.FullName;
        }

        public Type EditorType { get; set; }
        public string EditorAlias { get; set; }
    }

    public class CustomSectionRegistration
    {
        internal CustomSectionRegistration(Type sectionType)
        {
            SectionType = sectionType ?? throw new ArgumentNullException(nameof(sectionType));
            SectionAlias = sectionType.FullName;
        }

        public Type SectionType { get; set; }
        public string SectionAlias { get; set; }
    }

    public class Collection
    {
        internal string Name { get; set; }
        internal string Alias { get; set; }

        public List<Collection> Collections { get; set; } = new List<Collection>();

        internal List<EntityVariant>? SubEntityVariants { get; set; }
        internal EntityVariant EntityVariant { get; set; }

        internal EntityVariant GetEntityVariant(string? alias)
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
        internal EntityVariant GetEntityVariant(IEntity entity)
        {
            return SubEntityVariants?.FirstOrDefault(x => x.Type == entity.GetType())
                ?? EntityVariant;
        }

        internal Type RepositoryType { get; set; }
        internal IRepository Repository { get; set; }

        internal TreeView? TreeView { get; set; }

        internal ListView ListView { get; set; }
        internal ListEditor ListEditor { get; set; }

        internal NodeEditor NodeEditor { get; set; }

    }

    public interface ICollectionRoot
    {
        List<Collection> Collections { get; set; }
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

    internal class TreeView
    {
        internal EntityVisibilty EntityVisibility { get; set; }
        internal CollectionRootVisibility RootVisibility { get; set; }

        internal IExpressionMetadata Name { get; set; }
    }

    internal class ListView
    {
        internal ViewPane? ViewPane { get; set; }
        internal List<Button>? Buttons { get; set; }
    }

    internal class ListEditor
    {
        internal ListEditorType ListEditorType { get; set; }
        internal List<EditorPane> EditorPanes { get; set; }
        internal List<Button> Buttons { get; set; }
    }

    internal class SubCollectionListEditor : ListEditor
    {
        internal int Index { get; set; }

        internal string CollectionAlias { get; set; }
    }

    internal class NodeEditor
    {
        internal Type BaseType { get; set; }
        internal List<EditorPane> EditorPanes { get; set; }
        internal List<Button> Buttons { get; set; }
    }

    public class EntityVariant
    {
        public string Name { get; internal set; }
        public string Icon { get; internal set; }
        public Type Type { get; internal set; }
        public string Alias { get; internal set; }
    }

    internal class ViewPane
    {
        internal string Name { get; set; }

        internal List<Field> Fields { get; set; }
        internal List<Button> Buttons { get; set; }
    }

    internal class EditorPane
    {
        internal string? CustomAlias { get; set; }
        internal string? Label { get; set; }
        internal Type VariantType { get; set; }
        internal List<Button> Buttons { get; set; }
        internal List<Field> Fields { get; set; }
        internal List<SubCollectionListEditor> SubCollectionListEditors { get; set; }
    }

    internal abstract class Button
    {
        internal string ButtonId { get; set; }

        internal string Label { get; set; }
        internal string Icon { get; set; }
        internal bool ShouldConfirm { get; set; }
        internal bool IsPrimary { get; set; }

        internal List<Button> Buttons { get; set; }

        // TODO: how does this behave in custom buttons?
        internal object Metadata { get; set; }

        internal abstract CrudType GetCrudType();
        internal virtual bool IsCompatibleWithView(ViewContext viewContext) { return true; }
    }

    internal class DefaultButton : Button
    {
        internal DefaultButtonType DefaultButtonType { get; set; }

        internal override CrudType GetCrudType()
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
        internal override bool IsCompatibleWithView(ViewContext viewContext)
        {
            return DefaultButtonType.GetCustomAttribute<ActionsAttribute>().Usages?.Any(x => viewContext.Usage.HasFlag(x)) ?? false;
        }
    }

    internal class CustomButton : Button
    {
        internal string Alias { get; set; }

        internal IButtonActionHandler ActionHandler { get; set; }

        internal override CrudType GetCrudType()
        {
            return ActionHandler.GetCrudType();
        }
        internal override bool IsCompatibleWithView(ViewContext viewContext)
        {
            return ActionHandler.IsCompatibleWithView(viewContext);
        }
        internal Task HandleActionAsync(string? parentId, string? id, object? customData)
        {
            return ActionHandler.InvokeAsync(parentId, id, customData);
        }
    }


    internal class Field
    {
        internal int Index { get; set; }

        internal string Name { get; set; }
        internal string Description { get; set; }

        internal bool Readonly { get; set; } = true;

        internal EditorType DataType { get; set; } = EditorType.Readonly;

        internal IPropertyMetadata Property { get; set; }
        internal Type ValueMapperType { get; set; }

        internal OneToManyRelation? OneToManyRelation { get; set; }
    }

    internal class CustomField : Field
    {
        internal CustomField(Type customFieldType)
        {
            Alias = customFieldType?.FullName ?? throw new ArgumentNullException(nameof(customFieldType));
        }

        internal string Alias { get; set; }
    }

    // TODO: horrible names
    // TODO: structure is wrong..
    internal class OneToManyRelation
    {
    }

    internal class OneToManyCollectionRelation : OneToManyRelation
    {
        internal string CollectionAlias { get; set; }
        internal IPropertyMetadata IdProperty { get; set; }
        internal IExpressionMetadata DisplayProperty { get; set; }
    }

    internal class OneToManyDataProviderRelation : OneToManyRelation
    {
        internal Type DataCollectionType { get; set; }
    }
}
