using System.Collections.Generic;
using System.Linq;

namespace RapidCMS.Core.Models.ApiBridge.Request
{
    public class RelationContainerModel
    {
        public IEnumerable<RelationModel> Relations { get; set; } = Enumerable.Empty<RelationModel>();
    }
}
