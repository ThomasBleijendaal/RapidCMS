using System.Collections.Generic;

namespace RapidCMS.Core.Models.ApiBridge.Response;

public class EntitiesModel<TEntity>
{
    public IEnumerable<EntityModel<TEntity>> Entities { get; set; } = default!;
    public bool MoreDataAvailable { get; set; }
}
