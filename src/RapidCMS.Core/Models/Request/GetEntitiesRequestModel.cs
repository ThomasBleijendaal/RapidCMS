using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Models.Request
{
    internal class GetEntitiesRequestModel
    {
        internal UsageType UsageType { get; set; }
        internal string CollectionAlias { get; set; } = default!;
        internal ParentPath? ParentPath { get; set; }
        internal Query Query { get; set; } = default!;
    }
}
