using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RapidCMS.Common.Attributes;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Interfaces;

namespace RapidCMS.Common.Models
{
    public class Root : ICollectionRoot
    {
        private Dictionary<string, Collection> _collectionMap { get; set; } = new Dictionary<string, Collection>();

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

                var repo = serviceProvider.GetService(collection.RepositoryType);

                collection.Repository = (IRepository)repo;

                FindRepositoryForCollections(serviceProvider, collection.Collections);
            }
        }
    }

    public class Collection
    {
        public string Name { get; set; }
        public string Alias { get; set; }

        public List<Collection> Collections { get; set; } = new List<Collection>();

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
        public string Name { get; set; }
    }

    public enum ViewType
    {
        List,
        Tree
    }

    public class TreeView : View
    {
        public ViewType EntityViewType { get; set; }
        public ViewType SubEntityViewType { get; set; }

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
        public EditorPane<Field> EditorPane { get; set; }
        public List<Button> Buttons { get; set; }
    }

    public class NodeView : View
    {
        public List<ViewPane<NodeViewProperty>> ViewPanes { get; set; }
        public List<Button> Buttons { get; set; }
    }

    public class NodeEditor : Editor
    {
        public List<EditorPane<Field>> EditorPanes { get; set; }
        public List<Button> Buttons { get; set; }
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
        public List<Button> Buttons { get; set; }
        public List<T> Fields { get; set; }
    }

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

        public abstract bool IsCompatibleWithView(ViewContext viewContext);
    }

    public class DefaultButton : Button
    {
        public DefaultButtonType DefaultButtonType { get; set; }

        public override bool IsCompatibleWithView(ViewContext viewContext)
        {
            return DefaultButtonType.GetCustomAttribute<ActionsAttribute>().Usages?.Any(x => viewContext.Usage.HasFlag(x)) ?? false;
        }
    }

    public class Field
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public EditorType DataType { get; set; }
        
        public PropertyMetadata NodeProperty { get; set; }
        public IValueMapper ValueMapper { get; set; }
        public Type ValueMapperType { get; set; }
    }
}
