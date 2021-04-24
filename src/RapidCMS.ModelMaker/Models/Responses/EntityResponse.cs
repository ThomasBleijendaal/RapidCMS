using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.ModelMaker.Models.Responses
{
    public class EntityResponse<TEntity>
        where TEntity : IEntity
    {
        public TEntity? Entity { get; set; }
    }
}
