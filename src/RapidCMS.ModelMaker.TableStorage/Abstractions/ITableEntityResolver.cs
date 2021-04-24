using RapidCMS.ModelMaker.Abstractions.Entities;
using RapidCMS.ModelMaker.TableStorage.Entities;

namespace RapidCMS.ModelMaker.TableStorage.Abstractions
{
    internal interface ITableEntityResolver<TEntity>
        where TEntity : IModelMakerEntity
    {
        public ModelTableEntity ResolveTableEntity(TEntity entity);
        public TEntity? ResolveEntity(ModelTableEntity tableEntity);
    }
}
