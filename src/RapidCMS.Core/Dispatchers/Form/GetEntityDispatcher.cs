using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Request.Form;
using RapidCMS.Core.Repositories;

namespace RapidCMS.Core.Dispatchers.Form
{
    internal class GetEntityDispatcher : IPresentationDispatcher<GetEntityRequestModel, EditContext>
    {
        private readonly ISetupResolver<ICollectionSetup> _collectionResolver;
        private readonly IRepositoryResolver _repositoryResolver;
        private readonly IParentService _parentService;
        private readonly IConcurrencyService _concurrencyService;
        private readonly IAuthService _authService;
        private readonly IServiceProvider _serviceProvider;

        public GetEntityDispatcher(
            ISetupResolver<ICollectionSetup> collectionResolver,
            IRepositoryResolver repositoryResolver,
            IParentService parentService,
            IConcurrencyService concurrencyService,
            IAuthService authService,
            IServiceProvider serviceProvider)
        {
            _collectionResolver = collectionResolver;
            _repositoryResolver = repositoryResolver;
            _parentService = parentService;
            _concurrencyService = concurrencyService;
            _authService = authService;
            _serviceProvider = serviceProvider;
        }

        public async Task<EditContext> GetAsync(GetEntityRequestModel request)
        {
            if (string.IsNullOrWhiteSpace(request.Id) && (request.UsageType.HasFlag(UsageType.View) || request.UsageType.HasFlag(UsageType.Edit)))
            {
                throw new InvalidOperationException($"Cannot View/Edit Node when id is null");
            }
            if (!string.IsNullOrWhiteSpace(request.Id) && request.UsageType.HasFlag(UsageType.New))
            {
                throw new InvalidOperationException($"Cannot New Node when id is not null");
            }

            var collection = _collectionResolver.ResolveSetup(request.CollectionAlias);
            var repository = _repositoryResolver.GetRepository(collection);
            var repositoryContext = new RepositoryContext(request.CollectionAlias);

            var parent = await _parentService.GetParentAsync(request.ParentPath);

            var action = (request.UsageType & ~(UsageType.Node | UsageType.Root | UsageType.NotRoot)) switch
            {
                UsageType.View => () => repository.GetByIdAsync(repositoryContext, request.Id!, parent),
                UsageType.Edit => () => repository.GetByIdAsync(repositoryContext, request.Id!, parent),
                UsageType.New => () => repository.NewAsync(repositoryContext, parent, collection.GetEntityVariant(request.VariantAlias).Type)!,

                _ => default(Func<Task<IEntity?>>)
            };

            if (action == default)
            {
                throw new InvalidOperationException($"UsageType {request.UsageType} is invalid for this method");
            }

            var entity = await _concurrencyService.EnsureCorrectConcurrencyAsync(action);
            if (entity == null)
            {
                throw new Exception("Failed to get entity for given id(s)");
            }

            await _authService.EnsureAuthorizedUserAsync(request.UsageType, entity);

            return new EditContext(request.CollectionAlias, entity, parent, request.UsageType | UsageType.Node, _serviceProvider);
        }
    }
}
