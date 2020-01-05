using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Models.Request
{
    internal class GetEntityRequestModel
    {
        internal UsageType UsageType { get; set; }
        internal string CollectionAlias { get; set; } = default!;
        internal string? VariantAlias { get; set; }
        internal ParentPath? ParentPath { get; set; }
        internal string? Id { get; set; }
    }

    internal class GetEntitiesRequestModel
    {
        internal UsageType UsageType { get; set; }
        internal string CollectionAlias { get; set; } = default!;
        internal ParentPath? ParentPath { get; set; }
        internal Query Query { get; set; } = default!;
    }

    internal class GetChildEntitiesRequestModel : GetEntitiesRequestModel
    {
        internal ParentPath? ParentPath { get; set; }
    }

    internal class GetRelatedEntitiesRequestModel : GetEntitiesRequestModel
    {
        internal IRelated Related { get; set; } = default!;
    }

    internal class PersistEntityRequestModel
    {
        internal EditContext EditContext { get; set; } = default!;
        internal string ActionId { get; set; } = default!;
        internal object? CustomData { get; set; }
    }

    internal class PersistEntitiesRequestModel
    {
        internal UsageType UsageType { get; set; }
        internal string CollectionAlias { get; set; } = default!;
        internal ParentPath? ParentPath { get; set; }
        internal IEnumerable<EditContext> EditContexts { get; set; } = default!;
        internal string ActionId { get; set; } = default!;
        internal object? CustomData { get; set; }
    }

    internal class PersistRelatedEntitiesRequestModel : PersistEntitiesRequestModel
    {
        internal IRelated Related { get; set; } = default!;
    }
}
