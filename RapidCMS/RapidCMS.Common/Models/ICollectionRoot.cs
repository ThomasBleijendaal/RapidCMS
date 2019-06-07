using System.Collections.Generic;
using RapidCMS.Common.Models.Config;

#nullable enable

namespace RapidCMS.Common.Models
{
    public interface ICollectionRoot
    {
        List<CollectionConfig> Collections { get; set; }
    }
}
