using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Interactions;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Interactions
{
    internal class ButtonInteraction : IButtonInteraction
    {
        private readonly ICollectionResolver _collectionResolver;
        private readonly IAuthService _authService;

        public ButtonInteraction(
            ICollectionResolver collectionResolver,
            IAuthService authService)
        {
            _collectionResolver = collectionResolver;
            _authService = authService;
        }

        public async Task<CrudType> ValidateButtonInteractionAsync(IEditorButtonInteractionRequestModel request)
        {
            var collection = _collectionResolver.GetCollection(request.EditContext.CollectionAlias);

            var button = collection.FindButton(request.ActionId);
            if (button == null)
            {
                throw new Exception($"Cannot determine which button triggered action for collection {request.EditContext.CollectionAlias}");
            }

            await _authService.EnsureAuthorizedUserAsync(request.EditContext, button);

            if (button.RequiresValidForm(request.EditContext) && !request.EditContext.IsValid())
            {
                throw new InvalidEntityException();
            }

            var context = new ButtonContext(request.EditContext.Parent, request.CustomData);

            return await button.ButtonClickBeforeRepositoryActionAsync(request.EditContext, context);
        }

        public async Task CompleteButtonInteractionAsync(IEditorButtonInteractionRequestModel request)
        {
            var collection = _collectionResolver.GetCollection(request.EditContext.CollectionAlias);

            var button = collection.FindButton(request.ActionId);
            if (button == null)
            {
                throw new Exception($"Cannot determine which button triggered action for collection {request.EditContext.CollectionAlias}");
            }

            var context = new ButtonContext(request.EditContext.Parent, request.CustomData);

            await button.ButtonClickAfterRepositoryActionAsync(request.EditContext, context);
        }

        public async Task<CrudType> ValidateButtonInteractionAsync(IEditorInListInteractionRequestModel request)
        {
            var collection = _collectionResolver.GetCollection(request.ListContext.CollectionAlias);

            var button = collection.FindButton(request.ActionId);
            if (button == null)
            {
                throw new Exception($"Cannot determine which button triggered action for collection {request.ListContext.CollectionAlias}");
            }

            await _authService.EnsureAuthorizedUserAsync(request.EditContext, button);

            if (button.RequiresValidForm(request.EditContext) && !request.EditContext.IsValid())
            {
                throw new InvalidEntityException();
            }

            var context = new ButtonContext(request.EditContext.Parent, request.CustomData);

            return await button.ButtonClickBeforeRepositoryActionAsync(request.EditContext, context);
        }

        public async Task<(CrudType crudType, EntityVariantSetup? entityVariant)> ValidateButtonInteractionAsync(IListButtonInteractionRequestModel request)
        {
            var collection = _collectionResolver.GetCollection(request.ListContext.CollectionAlias);

            var button = collection.FindButton(request.ActionId);
            if (button == null)
            {
                throw new Exception($"Cannot determine which button triggered action for collection {request.ListContext.CollectionAlias}");
            }

            // TODO: this can cause an Update action be validated on the root, while it applies to the children (which is also checked)
            // this could lead to invalid rejection of action
            await _authService.EnsureAuthorizedUserAsync(request.ListContext.ProtoEditContext, button);

            var context = new ButtonContext(request.ListContext.Parent, request.CustomData);
            return (await button.ButtonClickBeforeRepositoryActionAsync(request.ListContext.ProtoEditContext, context), button.EntityVariant);
        }

        public async Task CompleteButtonInteractionAsync(IListButtonInteractionRequestModel request)
        {
            var collection = _collectionResolver.GetCollection(request.ListContext.CollectionAlias);

            var button = collection.FindButton(request.ActionId);
            if (button == null)
            {
                throw new Exception($"Cannot determine which button triggered action for collection {request.ListContext.CollectionAlias}");
            }

            var context = new ButtonContext(request.ListContext.Parent, request.CustomData);

            await button.ButtonClickAfterRepositoryActionAsync(request.ListContext.ProtoEditContext, context);
        }

        
    }
}
