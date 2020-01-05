using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.Commands;
using RapidCMS.Core.Models.Request;
using RapidCMS.Core.Models.Response;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Dispatchers
{

    internal class PersistEntityDispatcher : BaseDispatcher, IDispatcher<PersistEntityRequestModel, ViewCommandResponseModel>
    {
        public PersistEntityDispatcher(ICollectionResolver collectionResolver, IRepositoryResolver repositoryResolver, IParentService parentService, IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider, SemaphoreSlim semaphore) : base(collectionResolver, repositoryResolver, parentService, authorizationService, httpContextAccessor, serviceProvider, semaphore)
        {
        }

        public async Task<ViewCommandResponseModel> InvokeAsync(PersistEntityRequestModel request)
        {
            var collection = _collectionResolver.GetCollection(request.EditContext.CollectionAlias);
            var repository = _repositoryResolver.GetRepository(collection);

            var entityVariant = collection.GetEntityVariant(request.EditContext.Entity);

            var button = collection.FindButton(request.ActionId);
            if (button == null)
            {
                throw new Exception($"Cannot determine which button triggered action for collection {request.EditContext.CollectionAlias}");
            }

            await EnsureAuthorizedUserAsync(request.EditContext, button);
            EnsureValidEditContext(request.EditContext, button);

            ViewCommand viewCommand;

            var context = new ButtonContext(request.EditContext.Parent, request.CustomData);
            switch (await button.ButtonClickBeforeRepositoryActionAsync(request.EditContext, context))
            {
                case CrudType.View:
                    viewCommand = new NavigateCommand
                    {
                        Uri = NodeUri(Constants.View, request, entityVariant)
                    };
                    break;

                case CrudType.Edit:
                    viewCommand = new NavigateCommand
                    {
                        Uri = NodeUri(Constants.Edit, request, entityVariant)
                    };
                    break;

                case CrudType.Update:
                    await EnsureCorrectConcurrencyAsync(() => repository.UpdateAsync(request.EditContext));
                    viewCommand = new ReloadCommand(request.EditContext.Entity.Id!);
                    break;

                case CrudType.Insert:
                    var entity = await EnsureCorrectConcurrencyAsync(() => repository.InsertAsync(request.EditContext));
                    if (entity == null)
                    {
                        throw new Exception("Inserting the new entity failed.");
                    }
                    request.EditContext.SwapEntity(entity);

                    // TODO: make dependent of LIST or NODE
                    viewCommand = new NavigateCommand
                    {
                        Uri = NodeUri(Constants.Edit, request, entityVariant)
                    };
                    break;

                case CrudType.Delete:
                    await EnsureCorrectConcurrencyAsync(() => repository.DeleteAsync(request.EditContext.Entity.Id!, request.EditContext.Parent));
                    viewCommand = new NavigateCommand
                    {
                        Uri = CollectionUri(Constants.View, request)
                    };
                    break;

                case CrudType.None:
                    viewCommand = new NoOperationCommand();
                    break;

                case CrudType.Refresh:
                    viewCommand = new ReloadCommand();
                    break;

                case CrudType.Up:
                    viewCommand = new NavigateCommand
                    {
                        Uri = CollectionUri(collection.ListEditor == null ? Constants.View : Constants.Edit, request)
                    };
                    break;

                default:
                    throw new InvalidOperationException();
            }

            await button.ButtonClickAfterRepositoryActionAsync(request.EditContext, context);

            return new ViewCommandResponseModel
            {
                ViewCommand = viewCommand
            };
        }

        private static string CollectionUri(string action, PersistEntityRequestModel request)
        {
            return UriHelper.Collection(action, request.EditContext.CollectionAlias, request.EditContext.Parent?.GetParentPath());
        }

        private static string NodeUri(string action, PersistEntityRequestModel request, EntityVariantSetup entityVariant)
        {
            return UriHelper.Node(action, request.EditContext.CollectionAlias, entityVariant, request.EditContext.Parent?.GetParentPath(), request.EditContext.Entity.Id);
        }
    }
}
