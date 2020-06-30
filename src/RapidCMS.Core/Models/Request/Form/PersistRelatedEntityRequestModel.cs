using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.Request.Form
{
    public class PersistRelatedEntityRequestModel : PersistEntityRequestModel
    {
        public IRelated Related { get; set; } = default!;
    }
}
