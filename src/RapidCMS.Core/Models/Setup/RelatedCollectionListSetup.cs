using System;
using RapidCMS.Core.Enums;

namespace RapidCMS.Core.Models.Setup
{
    internal class RelatedCollectionListSetup
    {
        public RelatedCollectionListSetup(int index, string collectionAlias)
        {
            Index = index;
            CollectionAlias = collectionAlias ?? throw new ArgumentNullException(nameof(collectionAlias));
        }

        internal int Index { get; set; }
        internal string CollectionAlias { get; set; }

        internal UsageType SupportsUsageType { get; set; }
    }
}
