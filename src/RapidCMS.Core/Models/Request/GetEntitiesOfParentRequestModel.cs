using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Models.Request
{
    public class GetEntitiesOfParentRequestModel : GetEntitiesRequestModel
    {
        public ParentPath? ParentPath { get; set; }
    }
}
