using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.Request.Api;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Dispatchers.Api;

internal class GetEntityDispatcher : IPresentationDispatcher<GetEntityRequestModel, IEntity>
{
    private readonly ISetupResolver<EntityVariantSetup> _entityVariantResolver;
    private readonly IRepositoryResolver _repositoryResolver;
    private readonly IParentService _parentService;
    private readonly IAuthService _authService;

    public GetEntityDispatcher(
        ISetupResolver<EntityVariantSetup> entityVariantResolver,
        IRepositoryResolver repositoryResolver,
        IParentService parentService,
        IAuthService authService)
    {
        _entityVariantResolver = entityVariantResolver;
        _repositoryResolver = repositoryResolver;
        _parentService = parentService;
        _authService = authService;
    }

    public async Task<IEntity> GetAsync(GetEntityRequestModel request)
    {
        if (string.IsNullOrWhiteSpace(request.Subject.Id) && (request.UsageType.HasFlag(UsageType.View) || request.UsageType.HasFlag(UsageType.Edit)))
        {
            throw new InvalidOperationException($"Cannot View/Edit Node when id is null");
        }
        if (!string.IsNullOrWhiteSpace(request.Subject.Id) && request.UsageType.HasFlag(UsageType.New))
        {
            throw new InvalidOperationException($"Cannot New Node when id is not null");
        }

        var repository = _repositoryResolver.GetRepository(request.Subject.RepositoryAlias ?? throw new ArgumentNullException());

        var parent = await _parentService.GetParentAsync(ParentPath.TryParse(request.Subject.ParentPath));
        var entityVariant = request.Subject.VariantAlias == null ? default : await _entityVariantResolver.ResolveSetupAsync(request.Subject.VariantAlias);

        var action = (request.UsageType & ~(UsageType.Node)) switch
        {
            UsageType.View => () => repository.GetByIdAsync(request.Subject.Id!, new ViewContext(null, parent)),
            UsageType.Edit => () => repository.GetByIdAsync(request.Subject.Id!, new ViewContext(null, parent)),
            UsageType.New => () => repository.NewAsync(new ViewContext(null, parent), entityVariant?.Type)!,

            _ => default(Func<Task<IEntity?>>)
        };

        if (action == default)
        {
            throw new InvalidOperationException($"UsageType {request.UsageType} is invalid for this method");
        }

        var entity = await action.Invoke();
        if (entity == null)
        {
            throw new NotFoundException("Failed to get entity for given id");
        }

        await _authService.EnsureAuthorizedUserAsync(request.UsageType, entity);

        return entity;
    }
}
