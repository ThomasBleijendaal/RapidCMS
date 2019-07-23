using System;

namespace RapidCMS.Common.Messages
{
    public class CollectionUpdatedMessage
    {
        public CollectionUpdatedMessage(string collectionAlias)
        {
            CollectionAlias = collectionAlias ?? throw new ArgumentNullException(nameof(collectionAlias));
        }

        public string CollectionAlias { get; }
    }
}
