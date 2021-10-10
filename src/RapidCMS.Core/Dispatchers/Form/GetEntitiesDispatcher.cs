using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Request.Form;

namespace RapidCMS.Core.Dispatchers.Form
{
    internal class GetEntitiesDispatcher : IPresentationDispatcher<GetEntitiesRequestModel, ListContext>
    {
        private readonly ISetupResolver<ICollectionSetup> _collectionResolver;
        private readonly IRepositoryResolver _repositoryResolver;
        private readonly IDataViewResolver _dataViewResolver;
        private readonly IParentService _parentService;
        private readonly IConcurrencyService _concurrencyService;
        private readonly IAuthService _authService;
        private readonly IServiceProvider _serviceProvider;

        public GetEntitiesDispatcher(
            ISetupResolver<ICollectionSetup> collectionResolver,
            IRepositoryResolver repositoryResolver,
            IDataViewResolver dataViewResolver,
            IParentService parentService,
            IConcurrencyService concurrencyService,
            IAuthService authService,
            IServiceProvider serviceProvider)
        {
            _collectionResolver = collectionResolver;
            _repositoryResolver = repositoryResolver;
            _dataViewResolver = dataViewResolver;
            _parentService = parentService;
            _concurrencyService = concurrencyService;
            _authService = authService;
            _serviceProvider = serviceProvider;
        }

        public async Task<ListContext> GetAsync(GetEntitiesRequestModel request)
        {
            var collection = await _collectionResolver.ResolveSetupAsync(request.CollectionAlias);
            var variant = collection.GetEntityVariant(request.VariantAlias);
            var repository = _repositoryResolver.GetRepository(collection);

            var requestedEntityVariantIsDefaultVariant = variant.Alias == collection.EntityVariant.Alias;

            var parent = request is GetEntitiesOfParentRequestModel parentRequest ? await _parentService.GetParentAsync(parentRequest.ParentPath) : default;
            var relatedEntity = (request as GetEntitiesOfRelationRequestModel)?.Related;

            var protoEntity = await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.NewAsync(new ViewContext(collection.Alias, parent), collection.EntityVariant.Type));
            var newEntity = requestedEntityVariantIsDefaultVariant
                ? protoEntity
                : await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.NewAsync(new ViewContext(collection.Alias, parent), variant.Type));

            await _authService.EnsureAuthorizedUserAsync(request.UsageType, protoEntity);
            await _dataViewResolver.ApplyDataViewToViewAsync(request.View);

            var action = (request.UsageType & ~(UsageType.List | UsageType.Root | UsageType.NotRoot)) switch
            {
                UsageType.Add when relatedEntity != null => () => repository.GetAllNonRelatedAsync(new RelatedViewContext(relatedEntity!, collection.Alias, parent), request.View),
                _ when relatedEntity != null => () => repository.GetAllRelatedAsync(new RelatedViewContext(relatedEntity!, collection.Alias, parent), request.View),
                _ when relatedEntity == null => () => repository.GetAllAsync(new ViewContext(collection.Alias, parent), request.View),

                _ => default(Func<Task<IEnumerable<IEntity>>>)
            };

            if (action == default)
            {
                throw new InvalidOperationException($"UsageType {request.UsageType} is invalid for this method");
            }

            var existingEntities = await _concurrencyService.EnsureCorrectConcurrencyAsync(action);
            var protoEditContext = new FormEditContext(request.CollectionAlias, collection.RepositoryAlias, collection.EntityVariant.Alias, protoEntity, parent, request.UsageType | UsageType.List, collection.Validators, _serviceProvider);
            var newEditContext = new FormEditContext(request.CollectionAlias, collection.RepositoryAlias, variant.Alias, newEntity, parent, request.UsageType | UsageType.Node, collection.Validators, _serviceProvider);

            return new ListContext(
                request.CollectionAlias,
                protoEditContext,
                parent,
                request.UsageType,
                ConvertEditContexts(request, protoEditContext, newEditContext, existingEntities),
                _serviceProvider);
        }

        private List<FormEditContext> ConvertEditContexts(
            GetEntitiesRequestModel request,
            FormEditContext protoEditContext,
            FormEditContext newEditContext,
            IEnumerable<IEntity> existingEntities)
        {
            if (request.UsageType.HasFlag(UsageType.Add))
            {
                return existingEntities
                    .Select(ent => new FormEditContext(protoEditContext, ent, UsageType.Node | UsageType.Pick, _serviceProvider))
                    .ToList();
            }
            else if (request.UsageType.HasFlag(UsageType.Edit) || request.UsageType.HasFlag(UsageType.New))
            {
                var entities = existingEntities
                    .Select(ent => new FormEditContext(protoEditContext, ent, UsageType.Node | UsageType.Edit, _serviceProvider))
                    .ToList();

                if (request.UsageType.HasFlag(UsageType.New))
                {
                    entities.Insert(0, newEditContext);
                }

                return entities;
            }
            else if (request.UsageType.HasFlag(UsageType.View))
            {
                return existingEntities
                    .Select(ent => new FormEditContext(protoEditContext, ent, UsageType.Node | UsageType.View, _serviceProvider))
                    .ToList();
            }
            else
            {
                throw new NotImplementedException($"Failed to process {request.UsageType} for collection {request.CollectionAlias}");
            }
        }
    }
}
