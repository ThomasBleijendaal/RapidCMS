using System.Collections.Generic;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Interfaces.Setup
{
    internal interface ICollections
    {
        IEnumerable<CollectionSetup> Collections { get; }
    }
}
