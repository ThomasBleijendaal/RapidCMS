using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Repositories;

namespace RapidCMS.Core.Services.Parent
{
    internal class ParentService : IParentService
    {
        private readonly IRepositoryResolver _repositoryResolver;

        public ParentService(
            IRepositoryResolver repositoryResolver)
        {
            _repositoryResolver = repositoryResolver;
        }

        public async Task<IParent?> GetParentAsync(ParentPath? parentPath)
        {
            // TODO: heavily cache this. traversing the collection tree per call is very expensive

            var parent = default(ParentEntity);

            if (parentPath == null)
            {
                return parent;
            }

            foreach (var (collectionAlias, id) in parentPath)
            {
                var repositoryContext = new RepositoryContext(collectionAlias);
                var entity = await _repositoryResolver.GetRepository(collectionAlias).GetByIdAsync(repositoryContext, id, parent);
                if (entity == null)
                {
                    break;
                }

                parent = new ParentEntity(parent, entity, collectionAlias);
            }

            return parent;
        }
    }
}
