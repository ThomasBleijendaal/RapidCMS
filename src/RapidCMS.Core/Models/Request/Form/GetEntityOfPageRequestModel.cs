using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Models.Request.Form;

public class GetEntityOfPageRequestModel
{
    public string PageAlias { get; set; }
    public ParentPath? ParentPath { get; set; }
}
