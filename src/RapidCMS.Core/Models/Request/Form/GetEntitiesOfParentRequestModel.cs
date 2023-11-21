using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Models.Request.Form;

public class GetEntitiesOfParentRequestModel : GetEntitiesRequestModel
{
    public ParentPath? ParentPath { get; set; }
}
