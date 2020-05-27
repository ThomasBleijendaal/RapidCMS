using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.Core.Models.Response
{
    public class EntitiesResponseModel
    {
        public IEnumerable<IEntity> Entities { get; set; } = default!;

        public bool MoreDataAvailable { get; set; }
    }
}
