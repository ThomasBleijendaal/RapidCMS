using System;
using System.Collections.Generic;
using RapidCMS.Common.Enums;

namespace RapidCMS.Common.Models.Config
{
    public interface ICollectionRoot
    {
        List<CollectionConfig> Collections { get; set; }

        bool IsUnique(string alias);
    }
}
