using System;
using System.Collections.Generic;
using System.Text;
using RapidCMS.Common.Data;

namespace RapidCMS.Common.Models
{
    public class Root
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

                FindRepositoryForCollections(serviceProvider, collection.SubCollections);
            }
        }
    }

    public class Collection
    {
        public string Name { get; set; }
        public string Alias { get; set; }

        public Type RepositoryType { get; set; }
        public IRepository Repository { get; set; }

        public TreeView TreeView { get; set; }
        public List<Collection> SubCollections { get; set; }

        public ListView ListView { get; set; }
        public ListEditor ListEditor { get; set; }

        public EntityView EntityView { get; set; }
        public EntityEditor EntityEditor { get; set; }

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
        public List<Button> ButtonBar { get; set; }
    }

    public class Editor
    {
        public bool IsDefault { get; set; }
    }

    public class ListEditor : Editor
    {
        public List<EditorPane<Field>> EditorPanes { get; set; }
        public List<Button> ButtonBar { get; set; }
    }

    public class EntityView : View
    {
        public List<ViewPane<EntityViewProperty>> ViewPanes { get; set; }
        public List<Button> ButtonBar { get; set; }
    }

    public class EntityEditor : Editor
    {
        public List<EditorPane<Field>> EditorPanes { get; set; }
        public List<Button> ButtonBar { get; set; }
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
        public List<T> Fields { get; set; }
        public List<Button> Buttons { get; set; }
    }

    public class Property
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Func<object, object> Getter { get; set; }
        public Func<object, string> Formatter { get; set; }
    }

    public class ListViewProperty : Property
    {

    }

    public class EntityViewProperty : Property
    {

    }

    public class EntityListViewProperty : Property
    {

    }

    public class Button
    {
        public string Name { get; set; }
        public string Icon { get; set; }


    }

    public class Field
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public string DataType { get; set; }

        public Func<object, object> Getter { get; set; }
        public Action<object, object> Setter { get; set; }
        public Func<object, string> Formatter { get; set; }


    }
}
