using RapidCMS.Common.Enums;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Models.Config;
using RapidCMS.Example.Data;
using RapidCMS.Example.DataViews;
using RapidCMS.Repositories;

namespace RapidCMS.Example.Collections
{
    public static class MappedCollection
    {
        // CURD editor using a mapped repository
        public static void AddMappedCollection(this ICmsConfig config)
        {
            config.AddCollection<MappedEntity>("mapped", icon: "git-compare", "Mapped entities", collection =>
            {
                collection
                    .SetTreeView(EntityVisibilty.Hidden, x => x.Name)
                    .SetRepository<MappedInMemoryRepository<MappedEntity, DatabaseEntity>>()

                    // adding a data view builder allows you to have multiple tabs in the list editor, each with a different
                    // query associated with it
                    .SetDataViewBuilder<DatabaseEntityDataViewBuilder>()
                    .SetListEditor(editor =>
                    {
                        editor.AddDefaultButton(DefaultButtonType.Return);
                        editor.AddDefaultButton(DefaultButtonType.New);
                        
                        editor
                            .AddSection(section =>
                            {
                                section.AddField(x => x.Name);
                                section.AddField(x => x.Description);

                                section.AddDefaultButton(DefaultButtonType.SaveExisting);
                                section.AddDefaultButton(DefaultButtonType.SaveNew);
                            });
                    });
            });
        }
    }
}
