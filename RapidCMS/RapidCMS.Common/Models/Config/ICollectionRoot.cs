using System.Collections.Generic;


namespace RapidCMS.Common.Models.Config
{
    public interface ICollectionRoot
    {
        List<CollectionConfig> Collections { get; set; }

        bool IsUnique(string alias);
    }
}
