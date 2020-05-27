using System;
using System.Text;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.Request.Api;
using RapidCMS.Core.Repositories;

namespace RapidCMS.Core.Dispatchers.Api
{
    internal class GetEntityDispatcher : IPresentationDispatcher<GetEntityRequestModel, IEntity>
    {
        private readonly ISetupResolver<IEntityVariantSetup> _entityVariantResolver;
        private readonly IRepositoryResolver _repositoryResolver;
        private readonly IParentService _parentService;
        private readonly IAuthService _authService;

        public GetEntityDispatcher(
            ISetupResolver<IEntityVariantSetup> entityVariantResolver,
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

            var repository = _repositoryResolver.GetRepository(request.Subject.CollectionAlias ?? throw new ArgumentNullException());

            var parent = await _parentService.GetParentAsync(ParentPath.TryParse(request.Subject.ParentPath));
            var entityVariant = request.Subject.VariantAlias == null ? default : _entityVariantResolver.ResolveSetup(request.Subject.VariantAlias);

            var repositoryContext = new RepositoryContext(request.Subject.CollectionAlias);

            var action = (request.UsageType & ~(UsageType.Node | UsageType.Root | UsageType.NotRoot)) switch
            {
                UsageType.View => () => repository.GetByIdAsync(repositoryContext, request.Subject.Id!, parent),
                UsageType.Edit => () => repository.GetByIdAsync(repositoryContext, request.Subject.Id!, parent),
                UsageType.New => () => repository.NewAsync(repositoryContext, parent, entityVariant?.Type)!,

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
}
