using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Queries;
using RapidCMS.Core.Models.Results;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Handlers.QueryHandlers
{
    internal class GetEntitiesQueryHandler : IRequestHandler<GetEntitiesQuery, EntitiesResult>
    {
        private readonly ISetupResolver<CollectionSetup> _collectionResolver;
        private readonly IRepositoryResolver _repositoryResolver;
        private readonly IDataViewResolver _dataViewResolver;
        private readonly IParentService _parentService;
        private readonly IConcurrencyService _concurrencyService;
        private readonly IAuthService _authService;

        public GetEntitiesQueryHandler(
            ISetupResolver<CollectionSetup> collectionResolver,
            IRepositoryResolver repositoryResolver,
            IDataViewResolver dataViewResolver,
            IParentService parentService,
            IConcurrencyService concurrencyService,
            IAuthService authService)
        {
            _collectionResolver = collectionResolver;
            _repositoryResolver = repositoryResolver;
            _dataViewResolver = dataViewResolver;
            _parentService = parentService;
            _concurrencyService = concurrencyService;
            _authService = authService;
        }

        public async Task<EntitiesResult> HandleAsync(GetEntitiesQuery request)
        {
            var collection = await _collectionResolver.ResolveSetupAsync(request.CollectionAlias);
            var variant = collection.GetEntityVariant(request.VariantAlias);
            var repository = _repositoryResolver.GetRepository(collection);

            var requestedEntityVariantIsDefaultVariant = variant.Alias == collection.EntityVariant.Alias;

            var parent = request.ParentPath == null ? null : await _parentService.GetParentAsync(request.ParentPath);
            var relatedEntity = request.Related;

            var protoEntity = await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.NewAsync(new ViewContext(collection.Alias, parent), collection.EntityVariant.Type));
            var newEntity = requestedEntityVariantIsDefaultVariant
                ? protoEntity
                : await _concurrencyService.EnsureCorrectConcurrencyAsync(() => repository.NewAsync(new ViewContext(collection.Alias, parent), variant.Type));

            await _authService.EnsureAuthorizedUserAsync(request.UsageType, protoEntity);
            await _dataViewResolver.ApplyDataViewToViewAsync(request.View);

            var action = (request.UsageType & ~(UsageType.List)) switch
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
            
            var isRoot = parent == null || request.IsEmbedded;
            var protoEditContextUsage = request.UsageType | (isRoot ? UsageType.Root : UsageType.NotRoot);

            var entities = await _concurrencyService.EnsureCorrectConcurrencyAsync(action);

            if (request.UsageType.HasFlag(UsageType.New))
            {
                entities = entities.Insert(newEntity);
            }

#pragma warning disable S3358 // Ternary operators should not be nested
            var usageType = request.UsageType.HasFlag(UsageType.Add) ? UsageType.Node | UsageType.Pick
                : request.UsageType.HasFlag(UsageType.Edit) || request.UsageType.HasFlag(UsageType.New) ? UsageType.Node | UsageType.Edit
                : UsageType.Node | UsageType.View;
#pragma warning restore S3358 // Ternary operators should not be nested

            return new EntitiesResult(
                collection.Alias,
                collection.RepositoryAlias,
                new EntityResult(
                    collection.Alias,
                    collection.RepositoryAlias,
                    collection.EntityVariant.Alias,
                    protoEntity,
                    parent,
                    usageType,
                    collection.Validators),
                parent,
                protoEditContextUsage,
                entities.ToList(entity => new EntityResult(
                    collection.Alias,
                    collection.RepositoryAlias,
                    collection.EntityVariant.Alias,
                    entity,
                    parent,
                    usageType,
                    collection.Validators)));
        }
    }
}
