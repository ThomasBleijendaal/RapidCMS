using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Models.Request
{
    public class GetEntitiesRequestModel
    {
        public UsageType UsageType { get; set; }
        public string CollectionAlias { get; set; } = default!;
        public ParentPath? ParentPath { get; set; }
        public Query Query { get; set; } = default!;
    }
}
