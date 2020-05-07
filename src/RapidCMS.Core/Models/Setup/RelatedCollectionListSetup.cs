using System;

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
    }
}
