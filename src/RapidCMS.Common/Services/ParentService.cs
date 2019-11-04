using System.Threading.Tasks;
using RapidCMS.Common.Data;
using RapidCMS.Common.Providers;

namespace RapidCMS.Common.Services
{
    internal class ParentService : IParentService
    {
        private readonly ICollectionProvider _collectionProvider;

        public ParentService(ICollectionProvider collectionProvider)
        {
            _collectionProvider = collectionProvider;
        }

        public async Task<IParent?> GetParentAsync(ParentPath? parentPath)
        {
            // TODO: heavily cache this. traversing the collection tree per call is very expensive

            IParent? parent = default(ParentEntity);

            if (parentPath == null)
            {
                return parent;
            }

            foreach (var (collectionAlias, id) in parentPath)
            {
                var collection = _collectionProvider.GetCollection(collectionAlias);

                var entity = await collection.Repository.InternalGetByIdAsync(id, parent);
                if (entity == null)
                {
                    break;
                }

                parent = new ParentEntity(parent, entity);
            }

            return parent;
        }
    }
}
