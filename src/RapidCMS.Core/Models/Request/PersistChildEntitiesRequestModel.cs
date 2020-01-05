using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Models.Request
{
    internal class PersistChildEntitiesRequestModel : PersistEntitiesRequestModel
    {
        internal ParentPath? ParentPath { get; set; }
    }
}
