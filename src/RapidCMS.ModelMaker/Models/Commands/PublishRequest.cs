using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.ModelMaker.Models.Commands
{
    public class PublishRequest<TEntity>
        where TEntity : IEntity
    {
        public PublishRequest(TEntity entity)
        {
            Entity = entity;
        }

        public TEntity Entity { get; set; }
    }
}
