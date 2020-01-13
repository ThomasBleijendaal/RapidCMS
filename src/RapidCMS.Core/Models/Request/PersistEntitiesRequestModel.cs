using System.Collections.Generic;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Models.Request
{
    public class PersistEntitiesRequestModel
    {
        public UsageType UsageType { get; set; }
        public string CollectionAlias { get; set; } = default!;
        public IEnumerable<EditContext> EditContexts { get; set; } = default!;
        public string ActionId { get; set; } = default!;
        public object? CustomData { get; set; }
    }
}
