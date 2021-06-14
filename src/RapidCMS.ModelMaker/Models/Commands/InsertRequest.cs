using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.ModelMaker.Models.Commands
{
    public class InsertRequest<TEntity>
        where TEntity : IEntity
    {
        public InsertRequest(TEntity entity)
        {
            Entity = entity;
        }

        public TEntity Entity { get; set; }
    }
}
