using System.Collections.Generic;
using System.Linq;

namespace RapidCMS.Core.Models.ApiBridge.Request
{
    public class RelationModel
    {
        // TODO: protect this variable more
        public string RelatedTypeName { get; set; } = default!;
        public string PropertyName { get; set; } = default!;
        public IEnumerable<object> Elements { get; set; } = Enumerable.Empty<object>();
    }
}
