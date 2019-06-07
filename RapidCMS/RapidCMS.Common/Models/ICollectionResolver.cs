using RapidCMS.Common.Data;

#nullable enable

namespace RapidCMS.Common.Models
{
    // TODO: static root stuff is horrible
    // TODO: fix nullables

    public interface ICollectionResolver
    {
        IRepository? GetRepository(string collectionAlias);
    }
}
