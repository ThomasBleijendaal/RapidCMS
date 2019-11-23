using System.Linq;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Models.Config;
using RapidCMS.Example.Data;
using RapidCMS.Repositories;

namespace RapidCMS.Example.Collections
{
    public static class CountryCollection
    {
        // CRUD editor with support for one-to-many relation + validation
        public static void AddCountryCollection(this ICmsConfig config)
        {
            config.AddCollection<Country>("country", "Countries", collection =>
            {
                collection
                    .SetTreeView(x => x.Name)
                    .SetRepository<JsonRepository<Country>>()
                    .SetListView(view =>
                    {
                        view.AddDefaultButton(DefaultButtonType.New);

                        view
                            .AddRow(row =>
                            {
                                row.AddField(p => p.Id.ToString()).SetName("ID").SetType(DisplayType.Pre);
                                row.AddField(p => p.Name);

                                row.AddDefaultButton(DefaultButtonType.Edit);
                            });
                    })
                    .SetNodeEditor(editor =>
                    {
                        editor
                            .AddDefaultButton(DefaultButtonType.SaveExisting)
                            .AddDefaultButton(DefaultButtonType.SaveNew);

                        editor.AddSection(section =>
                        {
                            section.AddField(x => x.Name);

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
