using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Repositories;
using RapidCMS.Example.Shared.Components;
using RapidCMS.Example.Shared.Data;
using RapidCMS.Repositories;

namespace RapidCMS.Example.Shared.Collections
{
    public static class PersonCollection
    {
        // CRUD editor for simple POCO with recursive sub collections
        public static void AddPersonCollection(this ICmsConfig config)
        {
            // TODO: switch repository
            config.AddCollection<Person, BaseRepository<Person>>("person", "People", collection =>
            {
                collection
                    .SetTreeView(x => x.Name)
                    // this repository handles all the CRUD for this collection
                    // a list view is a table that displays a row (or multiple rows) of info per entity
                    .SetListView(view =>
                    {
                        // if new entities can be made using the CMS, include a New button so users can insert new entities
                        view.AddDefaultButton(DefaultButtonType.New);

                        // multiple rows can be added to display even more data
                        // only the first row will be used to generate headers
                        view.AddRow(row =>
                        {
                            // views always require you to specify strings, so the Id (an int) must be .ToString-ed.
                            // since this messes up the name of the column, you can set it to a nice name
                            row.AddField(p => p.Id.ToString()).SetName("ID");
                            row.AddField(p => p.Name);

                            // if an entity can be edited, include an Edit button so users can start editing entities
                            // the edit button in a list will always direct the user to the NodeEditor
                            row.AddDefaultButton(DefaultButtonType.Edit);
                        });
                    })
                    // a list editor is similair to a list view, but every column contains an editor so multiple entities can be edited in one go
                    // a list editor takes precedence over a list view, so when navigating to the Person collection, this view will be displayed
                    .SetListEditor(editor =>
                    {
                        editor.SetPageSize(2);

                        // adding Up to the button bar allows the user to get to the level above the current page (base upon the tree)
                        // this button will be hidden automatically when the user is at the root
                        editor.AddDefaultButton(DefaultButtonType.Up);

                        // in a list editor a New allows the user to add entities, within the list editor
                        editor.AddDefaultButton(DefaultButtonType.New);
                        editor.AddDefaultButton(DefaultButtonType.Return);

                        // adding a SaveExisting button to the ListEditor allows the user to bulk-save the entire list
                        // (only modified entities are touched)
                        editor.AddDefaultButton(DefaultButtonType.SaveExisting);

                        // allowing reordering so the user can shuffle the entities around and save them in a new order.
                        // the Repository must implement ReorderAsync
                        editor.AllowReordering(true);

                        // a list editor can be in the shape of a table, or a set of blocks, so these sections are either rows or blocks
                        editor.AddSection(row =>
                        {
                            // these fields will be the editors
                            row.AddField(p => p.Id).SetType(EditorType.Readonly);
                            row.AddField(p => p.Name);

                            // the SaveExisting button is only displayed when an entity is edited
                            row.AddDefaultButton(DefaultButtonType.SaveExisting, isPrimary: true);
                            // the SaveNew button is only displayed when an entity is inserted
                            row.AddDefaultButton(DefaultButtonType.SaveNew, isPrimary: true);

                            // the View button always directs the user to the NodeView
                            row.AddDefaultButton(DefaultButtonType.View);
                            // the Edit button always directs the user to the NodeEdit
                            row.AddDefaultButton(DefaultButtonType.Edit);
                        });
                    })
                    .SetNodeEditor(editor =>
                    {
                        // adding Up to the button bar allows the user to get to the level above the current page (base upon the tree)
                        editor.AddDefaultButton(DefaultButtonType.Up);

                        // just as in the ListEditor, SaveExisting only shows when the user is editing an existing entity, 
                        // and the SaveNew only when inserting an entity
                        editor.AddDefaultButton(DefaultButtonType.SaveExisting, isPrimary: true);
                        editor.AddDefaultButton(DefaultButtonType.SaveNew, isPrimary: true);

                        // if an entity can be deleted, add a button for it, so the user can delete the entity
                        editor.AddDefaultButton(DefaultButtonType.Delete);

                        // an node editor can have multiple sections, which are displayed as seperte blocks
                        editor.AddSection(section =>
                        {
                            // the DisableWhen expression is evaluated everytime any property of the entity is updated
                            // so this allows you to make response forms which show or hide parts based upon the entity and its state
                            section.AddField(x => x.Id).DisableWhen((person, state) => true);

                            // it is allowed to use DisplayType fields in Editors, so some readonly data can easily be displayed
                            section.AddField(x => x.Name).SetType(DisplayType.Label);
                            section.AddField(x => x.Email);
                        });

                        // you can even have sections specifically for an entity type.
                        // if the repository can return multiple types of entities (all derived from a shared base type), 
                        // sections can be made specifically for a type
                        editor.AddSection<Person>(section =>
                        {
                            // sections can have labels to make complex forms more understandable
                            section.SetLabel("Biography");

                            // sections can be hidden using VisibleWhen, based upon the entity or the state of that entity
                            // so users won't be confronted with editors that do not apply
                            section.VisibleWhen((person, state) => state == EntityState.IsExisting);

                            // there are many types of editors available, and even custom types can be used
                            section.AddField(x => x.Bio).SetType(EditorType.TextArea);
                        });

                        editor.AddSection(section =>
                        {
                            // relations with other entities, collections and repositories are first-class in RapidCMS
                            // so this field will allow the user to select an entity that is one level deeper in the person-tree
                            section.AddField(x => x.FavouriteChildId)
                                .SetName("Favourite child")
                                .SetType(EditorType.Select)
                                .SetCollectionRelation<Person>("person", config =>
                                {
                                    // this allows for configuring which property of the entity will make up the id for the element, and that value
                                    // is set to FavouriteChildId when the user selects an element
                                    config.SetElementIdProperty(x => x.Id);

                                    // because a single label is often not enough, you can add more properties to make the labels for each option
                                    config.SetElementDisplayProperties(x => x.Name, x => $"{x.Id} - {x.Email}");

                                    // sets the entity that is currently edited as parent for the repository to get elements for this field
                                    config.SetEntityAsParent();

                                    // any level from the tree can be picked:
                                    // Favouite sibling: config.SetRepositoryParent(x => x); 
                                    // Favouite parent: config.SetRepositoryParent(x => x.Parent); 
                                    // Favouite grand parent: config.SetRepositoryParent(x => x.Parent != null ? x.Parent.Parent : default); // ?. is not yet supported in expressions..
                                });
                        });
                    })
                    .SetNodeView(view =>
                    {
                        view.AddDefaultButton(DefaultButtonType.SaveExisting, isPrimary: true);
                        view.AddDefaultButton(DefaultButtonType.SaveNew, isPrimary: true);

                        view.AddDefaultButton(DefaultButtonType.Delete);

                        view.AddSection(section =>
                        {
                            // views also allow for customization of how the data should be displayed
                            // you can use the availablep DisplayType's, or create your own Razor components (must be derived from BaseDisplay)
                            section.AddField(x => x.Id.ToString()).SetName("ID").SetType(DisplayType.Pre);
                            section.AddField(x => x.Name).SetType(DisplayType.Label);
                            section.AddField(x => x.Email).SetType(typeof(EmailDisplay));
                        });

                        view.AddSection(section =>
                        {
                            section.AddField(x => x.Bio);
                        });
                    });

                collection.AddSelfAsRecursiveCollection();
            });
        }
    }
}
