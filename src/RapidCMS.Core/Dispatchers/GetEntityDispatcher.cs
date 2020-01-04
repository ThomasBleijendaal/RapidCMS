using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Request;
using RapidCMS.Core.Models.Response;

namespace RapidCMS.Core.Dispatchers
{
    internal class GetEntityDispatcher : BaseDispatcher, IDispatcher<GetEntityRequestModel, EntityResponseModel>
    {
        public GetEntityDispatcher(ICollectionResolver collectionResolver, IRepositoryResolver repositoryResolver, IParentService parentService, IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider, SemaphoreSlim semaphore) : base(collectionResolver, repositoryResolver, parentService, authorizationService, httpContextAccessor, serviceProvider, semaphore)
        {
        }

        public async Task<EntityResponseModel> InvokeAsync(GetEntityRequestModel request)
        {
            if (string.IsNullOrWhiteSpace(request.Id) && (request.UsageType.HasFlag(UsageType.View) || request.UsageType.HasFlag(UsageType.Edit)))
            {
                throw new InvalidOperationException($"Cannot View Node when id is null");
            }

            var collection = _collectionResolver.GetCollection(request.CollectionAlias);
            var repository = _repositoryResolver.GetRepository(collection);

            var parent = await _parentService.GetParentAsync(request.ParentPath);

            var action = (request.UsageType & ~(UsageType.Node | UsageType.Root | UsageType.NotRoot)) switch
            {
                UsageType.View => () => repository.GetByIdAsync(request.Id!, parent),
                UsageType.Edit => () => repository.GetByIdAsync(request.Id!, parent),
                UsageType.New => () => repository.NewAsync(parent, collection.GetEntityVariant(request.VariantAlias).Type)!,

                _ => default(Func<Task<IEntity?>>)
            };

            if (action == default)
            {
                throw new InvalidOperationException($"UsageType {request.UsageType} is invalid for this method");
            }

            var entity = await EnsureCorrectConcurrencyAsync(action);
            if (entity == null)
            {
                throw new Exception("Failed to get entity for given id(s)");
            }

            await EnsureAuthorizedUserAsync(request.UsageType, entity);

            return new EntityResponseModel
            {
                EditContext = new EditContext(request.CollectionAlias, entity, parent, request.UsageType | UsageType.Node, _serviceProvider)
            };
        }

        private object Func<T>()
        {
            throw new NotImplementedException();
        }
    }
}
