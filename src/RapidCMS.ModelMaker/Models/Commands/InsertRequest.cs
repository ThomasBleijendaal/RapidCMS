using RapidCMS.ModelMaker.Abstractions.Entities;

namespace RapidCMS.ModelMaker.Models.Commands
{
    public class InsertRequest<TEntity>
        where TEntity : IModelMakerEntity
    {
        public InsertRequest(TEntity entity)
        {
            Entity = entity;
        }

        public TEntity Entity { get; set; }
    }
}
