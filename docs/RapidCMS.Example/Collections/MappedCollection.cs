using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Enums;
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
            config.AddCollection<MappedEntity, MappedInMemoryRepository<MappedEntity, DatabaseEntity>>("mapped", icon: "git-compare", "Mapped entities", collection =>
            {
                collection
                    .SetTreeView(EntityVisibilty.Hidden, x => x.Name)
                    
                    // adding a data view builder allows you to have multiple tabs in the list editor, each with a different
                    // query associated with it
                    .SetDataViewBuilder<DatabaseEntityDataViewBuilder, DatabaseEntity>()
                    .SetListEditor(editor =>
                    {
                        editor.AddDefaultButton(DefaultButtonType.Return);
                        editor.AddDefaultButton(DefaultButtonType.New);
                        
                        editor
                            .AddSection(section =>
                            {
                                // since the repository is mapped, the OrderBy query send to the IQueryable of the repository
                                // is based on DatabaseEntity, and not MappedEntity. because of this, the overload of SetOrderByExpression
                                // is used by which you can specify the type of entity
                                section.AddField(x => x.Name)
                                    .SetOrderByExpression<DatabaseEntity, string?>(x => x.Name);
                                section.AddField(x => x.Description)
                                    .SetOrderByExpression<DatabaseEntity, string?>(x => x.Description);

                                section.AddDefaultButton(DefaultButtonType.SaveExisting);
                                section.AddDefaultButton(DefaultButtonType.SaveNew);
                            });
                    });
            });
        }
    }
}
