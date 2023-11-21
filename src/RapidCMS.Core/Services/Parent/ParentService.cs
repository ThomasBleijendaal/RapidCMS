using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Data;

namespace RapidCMS.Core.Services.Parent;

internal class ParentService : IParentService
{
    private readonly IRepositoryResolver _repositoryResolver;
    private readonly IConcurrencyService _concurrencyService;

    public ParentService(
        IRepositoryResolver repositoryResolver,
        IConcurrencyService concurrencyService)
    {
        _repositoryResolver = repositoryResolver;
        _concurrencyService = concurrencyService;
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
            var entity = await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repo.GetByIdAsync(id, new ViewContext(null, parent)));
            if (entity == null)
            {
                break;
            }

            parent = new ParentEntity(parent, entity, repositoryAlias);
        }

        return parent;
    }
}
