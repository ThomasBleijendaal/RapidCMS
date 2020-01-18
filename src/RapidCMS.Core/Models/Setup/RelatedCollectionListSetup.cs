using RapidCMS.Core.Models.Config;

namespace RapidCMS.Core.Models.Setup
{
    internal class RelatedCollectionListSetup
    {
        internal RelatedCollectionListSetup(CollectionListConfig subCollection)
        {
            Index = subCollection.Index;
            CollectionAlias = subCollection.CollectionAlias;
        }

        internal int Index { get; set; }
        internal string CollectionAlias { get; set; }
    }
}
