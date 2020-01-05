using System.Collections.Generic;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Models.Request
{
    internal class PersistEntitiesRequestModel
    {
        internal UsageType UsageType { get; set; }
        internal string CollectionAlias { get; set; } = default!;
        internal IEnumerable<EditContext> EditContexts { get; set; } = default!;
        internal string ActionId { get; set; } = default!;
        internal object? CustomData { get; set; }
    }
}
