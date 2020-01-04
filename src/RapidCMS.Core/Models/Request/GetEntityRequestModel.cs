using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Enums;
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
}
