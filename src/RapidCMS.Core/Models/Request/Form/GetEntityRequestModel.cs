using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Models.Request.Form
{
    public class GetEntityRequestModel
    {
        public UsageType UsageType { get; set; }
        public string CollectionAlias { get; set; } = default!;
        public string VariantAlias { get; set; } = default!;
        public ParentPath? ParentPath { get; set; }
        public string? Id { get; set; }
    }
}
