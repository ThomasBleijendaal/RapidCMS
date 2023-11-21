using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.Request.Form;

namespace RapidCMS.Core.Dispatchers.Form;

internal class GetEntityOfPageDispatcher : IPresentationDispatcher<GetEntityOfPageRequestModel, PageContext>
{
    private readonly IRepositoryResolver _repositoryResolver;
    private readonly IParentService _parentService;
    private readonly IConcurrencyService _concurrencyService;
    private readonly IAuthService _authService;
    
    public GetEntityOfPageDispatcher(
        IRepositoryResolver repositoryResolver,
        IParentService parentService,
        IConcurrencyService concurrencyService,
        IAuthService authService)
    {
        _repositoryResolver = repositoryResolver;
        _parentService = parentService;
        _concurrencyService = concurrencyService;
        _authService = authService;
    }

    public async Task<PageContext> GetAsync(GetEntityOfPageRequestModel request)
    {
        var (newParentPath, repositoryAlias, id) = ParentPath.RemoveLevel(request.ParentPath);

        if (repositoryAlias == null || id == null)
        {
            return new PageContext(request.PageAlias, null, null);
        }

        var repository = _repositoryResolver.GetRepository(repositoryAlias);

        var parent = await _parentService.GetParentAsync(newParentPath);

        var entity = await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.GetByIdAsync(id, new ViewContext(request.PageAlias, parent)));
        if (entity == null)
        {
            throw new Exception("Failed to get entity for given id(s)");
        }

        await _authService.EnsureAuthorizedUserAsync(UsageType.View, entity);

        return new PageContext(request.PageAlias, entity, parent);
    }
}
