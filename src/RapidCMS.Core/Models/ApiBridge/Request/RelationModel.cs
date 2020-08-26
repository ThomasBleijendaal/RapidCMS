using System.Collections.Generic;
using System.Linq;

namespace RapidCMS.Core.Models.ApiBridge.Request
{
    public class RelationModel
    {
        public string VariantAlias { get; set; } = default!;
        public string PropertyName { get; set; } = default!;
        public IEnumerable<object> Elements { get; set; } = Enumerable.Empty<object>();
    }
}
