using System;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;

namespace RapidCMS.Core.Forms
{
    internal class ViewContext : IViewContext
    {
        public static ViewContext Default => new(null, default);

        public ViewContext(string? collectionAlias, IParent? parent)
        {
            CollectionAlias = collectionAlias;
            Parent = parent;
        }

        public string? CollectionAlias { get; }

        public IParent? Parent { get; }
    }
}
