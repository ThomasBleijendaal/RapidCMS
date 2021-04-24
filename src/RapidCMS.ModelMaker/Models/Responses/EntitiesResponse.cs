using System.Collections.Generic;
using System.Linq;

namespace RapidCMS.ModelMaker.Models.Responses
{
    public class EntitiesResponse<TEntity>
    {
        public IEnumerable<TEntity> Entities { get; set; } = Enumerable.Empty<TEntity>();
    }
}
