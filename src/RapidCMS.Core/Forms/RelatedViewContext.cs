using System;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;

namespace RapidCMS.Core.Forms
{
    internal class RelatedViewContext : ViewContext, IRelatedViewContext
    {
        public RelatedViewContext(IRelated related, string? collectionAlias, IParent? parent) : base(collectionAlias, parent)
        {
            Related = related ?? throw new ArgumentNullException(nameof(related));
        }

        public IRelated Related { get; }
    }
}
