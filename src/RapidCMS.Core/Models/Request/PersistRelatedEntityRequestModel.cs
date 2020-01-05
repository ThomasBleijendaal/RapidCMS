using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.Request
{
    internal class PersistRelatedEntityRequestModel : PersistEntityRequestModel
    {
        internal IRelated Related { get; set; } = default!;
    }
}
