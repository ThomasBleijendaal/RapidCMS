using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.ModelMaker.Models.Commands
{
    public class UpdateRequest<TEntity>
        where TEntity : IEntity
    {
        public UpdateRequest(TEntity entity)
        {
            Entity = entity;
        }

        public TEntity Entity { get; set; }
    }
}
