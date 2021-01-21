using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Services.Parent
{
    internal class ParentService : IParentService
    {
        private readonly IRepositoryResolver _repositoryResolver;

        public ParentService(IRepositoryResolver repositoryResolver)
        {
            _repositoryResolver = repositoryResolver;
        }

        public async Task<IParent?> GetParentAsync(ParentPath? parentPath)
        {
            var parent = default(ParentEntity);

            if (parentPath == null)
            {
                return parent;
            }

            foreach (var (repositoryAlias, id) in parentPath)
            {
                var repo = _repositoryResolver.GetRepository(repositoryAlias);
                var entity = await repo.GetByIdAsync(id, parent);
                if (entity == null)
                {
                    break;
                }

                parent = new ParentEntity(parent, entity, repositoryAlias);
            }

            return parent;
        }
    }
}
