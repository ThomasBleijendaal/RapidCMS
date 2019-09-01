using System.Collections.Generic;
using RapidCMS.Common.Models;

namespace RapidCMS.Common.Data
{
    public interface ICollectionProvider
    {
        Collection GetCollection(string alias);
        IEnumerable<Collection> GetAllCollections();
    }
}
