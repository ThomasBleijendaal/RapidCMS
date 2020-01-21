using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.Request
{
    public class GetEntitiesOfRelationRequestModel : GetEntitiesRequestModel
    {
        public IRelated Related { get; set; } = default!;
    }
}
