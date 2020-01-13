using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Models.Request
{
    public class PersistChildEntitiesRequestModel : PersistEntitiesRequestModel
    {
        public ParentPath? ParentPath { get; set; }
    }
}
