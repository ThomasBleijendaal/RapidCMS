using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Enums;
using RapidCMS.Example.Data;
using RapidCMS.Repositories;

namespace RapidCMS.Example.Collections
{
    public static class ConventionCollection
    {
        // CRUD editor for simple POCO based on conventions 
        public static void AddConventionCollection(this ICmsConfig config)
        {
            config.AddCollection<ConventionalPerson, JsonRepository<ConventionalPerson>>("person-convention", "People (by convention)", collection =>
            {
                collection.SetTreeView(x => x.Name); // TODO: move into convention
                collection.ConfigureByConvention(CollectionConvention.ListViewNodeEditor);
            });
        }
    }
}
