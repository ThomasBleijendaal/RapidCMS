using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Models.Request
{
    public class GetEntityRequestModel
    {
        public UsageType UsageType { get; set; }
        public string CollectionAlias { get; set; } = default!;
        public string? VariantAlias { get; set; }
        public ParentPath? ParentPath { get; set; }
        public string? Id { get; set; }
    }
}
