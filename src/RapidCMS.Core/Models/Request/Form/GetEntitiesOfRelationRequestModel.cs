using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.Request.Form
{
    public class GetEntitiesOfRelationRequestModel : GetEntitiesRequestModel
    {
        public IRelated Related { get; set; } = default!;
    }
}
