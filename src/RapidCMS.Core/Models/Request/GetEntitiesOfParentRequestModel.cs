using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Models.Request
{
    public class GetEntitiesOfParentRequestModel : GetEntitiesRequestModel
    {
        public ParentPath? ParentPath { get; set; }
    }

    public class GetEntitiesOfRelationRequestModel : GetEntitiesRequestModel
    {
        public IRelated Related { get; set; }
    }

    public class GetEntitiesRequestModel
    {
        public UsageType UsageType { get; set; }
        public string CollectionAlias { get; set; } = default!;
        public Query Query { get; set; } = default!;
    }
}
