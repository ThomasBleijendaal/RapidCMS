using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Models.Request;
using RapidCMS.Core.Models.Response;

namespace RapidCMS.Core.Dispatchers
{
    internal class GetEntitiesDispatcher : BaseDispatcher, IDispatcher<GetEntitiesRequestModel, EntitiesResponseModel>
    {
        public GetEntitiesDispatcher(ICollectionResolver collectionResolver, IRepositoryResolver repositoryResolver, IParentService parentService, IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider, SemaphoreSlim semaphore) : base(collectionResolver, repositoryResolver, parentService, authorizationService, httpContextAccessor, serviceProvider, semaphore)
        {
        }

        public async Task<EntitiesResponseModel> InvokeAsync(GetEntitiesRequestModel request)
        {
            var collection = _collectionResolver.GetCollection(request.CollectionAlias);
            var repository = _repositoryResolver.GetRepository(collection);

            var dummyEntity = await EnsureCorrectConcurrencyAsync(() => repository.NewAsync(null, collection.EntityVariant.Type));
            await EnsureAuthorizedUserAsync(request.UsageType, dummyEntity);

            await collection.ProcessDataViewAsync(request.Query, _serviceProvider);

            var parent = await _parentService.GetParentAsync(request.ParentPath);

            var existingEntities = await EnsureCorrectConcurrencyAsync(() => repository.GetAllAsync(parent, request.Query));

            var rootEditContext = await GetNewEditContextAsync(request.UsageType, request.CollectionAlias, parent);

            return new EntitiesResponseModel
            {
                EditContexts = ConvertEditContexts(
                    request.UsageType,
                    request.CollectionAlias,
                    rootEditContext,
                    existingEntities,
                    parent)
            };
        }
    }
}
