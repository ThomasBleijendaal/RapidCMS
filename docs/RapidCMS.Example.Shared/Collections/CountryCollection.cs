using System.Linq;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Repositories;
using RapidCMS.Example.Shared.Data;

namespace RapidCMS.Example.Shared.Collections
{
    public static class CountryCollection
    {
        // CRUD editor with support for one-to-many relation + validation
        public static void AddCountryCollection(this ICmsConfig config)
        {
            config.AddCollection<Country, BaseRepository<Country>>("country", "Countries", collection =>
            {
                collection
                    // Set showEntities to true to have this collection to fold open on default
                    .SetTreeView(x => x.Name, showEntitiesOnStartup: true)
                    .SetListView(view =>
                    {
                        view.AddDefaultButton(DefaultButtonType.New);

                        view.SetPageSize(2);

                        view
                            .AddRow(row =>
                            {
                                row.AddField(p => p.Id.ToString())
                                    .SetName("ID").SetType(DisplayType.Pre);

                                // by specifying SetOrderExpression you can allow the user to sort the data on that column
                                // giving it a default order will cause this column to be ordered when the user opens the view
                                row.AddField(p => p.Name)
                                    .SetOrderByExpression(p => p.Name, OrderByType.Ascending);

                                // multiple columns can be ordered at once, and the OrderBys stack from left to right,
                                // so the Countries in this example will always be first ordered by Name, then by Metadata.Continent
                                row.AddField(p => p.Metadata.Continent)
                                    .SetOrderByExpression(p => p.Metadata.Continent);

                                row.AddDefaultButton(DefaultButtonType.Edit);
                            });
                    })
                    .SetNodeEditor(editor =>
                    {
                        editor
                            // the Up button allows users to get one level up (based upon the tree)
                            .AddDefaultButton(DefaultButtonType.Up, "Back to list", "list")
                            .AddDefaultButton(DefaultButtonType.SaveExisting)
                            .AddDefaultButton(DefaultButtonType.SaveNew);

                        editor.AddSection(section =>
                        {
                            section.AddField(x => x.Name);
                            section.AddField(x => x.Metadata.Continent);

                            // this property contains a list of people it is related to
                            // you can see it as a ICollection<TRelated> property in EF Core

                            // because this property has an RelationValidationAttribute, the CMS first checks if the
                            // selected values are valid, before allowing the user to save this entity.
                            // in this example, the user may only select up to two People per Country

                            section.AddField(x => x.People)
                                // a multi-select is a list of checkboxes
                                .SetType(EditorType.MultiSelect)
                                // this binds the PersonCollection to this collection
                                // the CMS must know what the foreign entity and key is, you need to specify it
                                .SetCollectionRelation<Person, int>(
                                    // this selects which values are used as selected values 
                                    people => people.Select(p => p.Id),
                                    // alias of the related collection
                                    "person",
                                    // this callback allows you to specify how the multi-select should look like
                                    relation =>
                                    {
                                        relation
                                            // when the user selects an element, the value that is used as Id is used
                                            // to set the value of the property
                                            .SetElementIdProperty(x => x.Id)
                                            .SetElementDisplayProperties(x => x.Name, x => x.Email);
                                    });
                        });
                    });
            });
        }
    }
}
