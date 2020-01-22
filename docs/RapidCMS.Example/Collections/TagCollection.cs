using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Enums;
using RapidCMS.Example.Components;
using RapidCMS.Example.Data;
using RapidCMS.Example.Handlers;
using RapidCMS.Repositories;

namespace RapidCMS.Example.Collections
{
    public static class TagCollection
    {
        public static void AddTagCollection(this ICmsConfig config)
        {
            config.AddCollection<TagGroup, JsonRepository<TagGroup>>("taggroup", "Tag groups", collection =>
            {
                collection
                    .SetTreeView(EntityVisibilty.Hidden, x => x.Name)
                    .SetListEditor(editor =>
                    {
                        editor.SetListType(ListType.Block);

                        editor.SetSearchBarVisibility(false);

                        editor.AddDefaultButton(DefaultButtonType.Return);
                        editor.AddDefaultButton(DefaultButtonType.New);

                        editor
                            .AddSection(section =>
                            {
                                section.AddDefaultButton(DefaultButtonType.SaveExisting);
                                section.AddDefaultButton(DefaultButtonType.SaveNew);

                                section.AddField(x => x.Name);

                                // it is possible to view or edit an subcollection from the parent
                                // when adding a subcollection in an Editor will have to be a ListEditor while
                                // adding a subcollection in a View will be a ListView.

                                // the entity of this editor will be passed in as IParent in the repository of the 
                                // sub collection, making it possible to access the parents properties in the childrens repository
                                section.AddSubCollectionList<Tag, JsonRepository<Tag>>(config =>
                                {
                                    config.SetListEditor(editor =>
                                    {
                                        editor.SetSearchBarVisibility(false);

                                        editor.AddDefaultButton(DefaultButtonType.Return);
                                        editor.AddDefaultButton(DefaultButtonType.New);

                                        editor.AddSection(section =>
                                        {
                                            section.AddDefaultButton(DefaultButtonType.SaveExisting);
                                            section.AddDefaultButton(DefaultButtonType.SaveNew);

                                            // add custom buttons and action handlers using the following handler
                                            section.AddCustomButton<RandomNameActionHandler>(typeof(CustomButton), "Create name", "add-circle");

                                            section.AddField(x => x.Name);
                                        });
                                    });
                                });

                                // if you want to reuse a collection in multiple views, you can also reference it by alias
                                // if you comment out the AddSubCollectionList above this comment, and enable the AddSubCollectionList below, 
                                // the editor will work identical, but now the collection with alias "tag" can be used for multiple things
                                
                                // section.AddSubCollectionList("tag");
                            });
                    });

                // any collection can be added as subcollection, even collections based upon totally difference repositories
                // this lets you mix repositories which are based upon totally different databases easily
                collection
                    .AddSubCollection<Tag, JsonRepository<Tag>>("tag", "Tags", subCollection =>
                    {
                        subCollection
                            .SetListEditor(editor =>
                            {
                                editor.SetListType(ListType.Table);

                                editor.SetSearchBarVisibility(false);

                                editor.AddDefaultButton(DefaultButtonType.Return);
                                editor.AddDefaultButton(DefaultButtonType.New);

                                editor.AddSection(section =>
                                {
                                    section.AddDefaultButton(DefaultButtonType.SaveExisting);
                                    section.AddDefaultButton(DefaultButtonType.SaveNew);

                                    // add custom buttons and action handlers using the following handler
                                    section.AddCustomButton<RandomNameActionHandler>(typeof(CustomButton), "Create name", "add-circle");

                                    section.AddField(x => x.Name);
                                });
                            });
                    });
            });
        }
    }
}
