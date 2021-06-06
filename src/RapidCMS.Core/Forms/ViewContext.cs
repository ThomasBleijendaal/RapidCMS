using System;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;

namespace RapidCMS.Core.Forms
{
    internal class ViewContext : IViewContext
    {
        public static ViewContext Default => new("", default);

        public ViewContext(string collectionAlias, IParent? parent)
        {
            CollectionAlias = collectionAlias ?? throw new ArgumentNullException(nameof(collectionAlias));
            Parent = parent;
        }

        public string CollectionAlias { get; }

        public IParent? Parent { get; }
    }
}
