using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Models.UI
{
    public class SubCollectionUI : ElementUI
    {
        internal SubCollectionUI(SubCollectionListSetup subCollection) : base((x, y) => true, (x, y) => false) // TODO: fixed lambdas
        {
            CollectionAlias = subCollection.CollectionAlias;
        }

        public string CollectionAlias { get; private set; }
    }
}
