using System;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Abstractions.Interactions;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Interactions
{
    internal class ButtonInteraction : IButtonInteraction
    {
        private readonly ISetupResolver<ICollectionSetup> _collectionResolver;
        private readonly IButtonActionHandlerResolver _buttonActionHandlerResolver;
        private readonly IAuthService _authService;

        public ButtonInteraction(
            ISetupResolver<ICollectionSetup> collectionResolver,
            IButtonActionHandlerResolver buttonActionHandlerResolver,
            IAuthService authService)
        {
            _collectionResolver = collectionResolver;
            _buttonActionHandlerResolver = buttonActionHandlerResolver;
            _authService = authService;
        }

        public async Task<CrudType> ValidateButtonInteractionAsync(IEditorButtonInteractionRequestModel request)
        {
            var (handler, button) = await FindButtonHandlerAsync(request.EditContext.CollectionAlias, request.ActionId);

            await _authService.EnsureAuthorizedUserAsync(request.EditContext, button);

            if (handler.RequiresValidForm(button, request.EditContext) && !request.EditContext.IsValid())
            {
                throw new InvalidEntityException();
            }

            var context = new ButtonContext(request.EditContext.Parent, request.CustomData);

            return await handler.ButtonClickBeforeRepositoryActionAsync(button, request.EditContext, context);
        }

        public async Task CompleteButtonInteractionAsync(IEditorButtonInteractionRequestModel request)
        {
            var (handler, button) = await FindButtonHandlerAsync(request.EditContext.CollectionAlias, request.ActionId);
            
            var context = new ButtonContext(request.EditContext.Parent, request.CustomData);

            await handler.ButtonClickAfterRepositoryActionAsync(button, request.EditContext, context);
        }

        public async Task<CrudType> ValidateButtonInteractionAsync(IEditorInListInteractionRequestModel request)
        {
            var (handler, button) = await FindButtonHandlerAsync(request.ListContext.CollectionAlias, request.ActionId);

            await _authService.EnsureAuthorizedUserAsync(request.EditContext, button);

            if (handler.RequiresValidForm(button, request.EditContext) && !request.EditContext.IsValid())
            {
                throw new InvalidEntityException();
            }

            var context = new ButtonContext(request.EditContext.Parent, request.CustomData);

            return await handler.ButtonClickBeforeRepositoryActionAsync(button, request.EditContext, context);
        }

        public async Task<(CrudType crudType, IEntityVariantSetup? entityVariant)> ValidateButtonInteractionAsync(IListButtonInteractionRequestModel request)
        {
            var (handler, button) = await FindButtonHandlerAsync(request.ListContext.CollectionAlias, request.ActionId);

            // NOTE: this might check too much or reject because of the wrong reasons.
            await _authService.EnsureAuthorizedUserAsync(request.ListContext.ProtoEditContext, button);

            var context = new ButtonContext(request.ListContext.Parent, request.CustomData);
            return (await handler.ButtonClickBeforeRepositoryActionAsync(button, request.ListContext.ProtoEditContext, context), button.EntityVariant);
        }

        public async Task CompleteButtonInteractionAsync(IListButtonInteractionRequestModel request)
        {
            var (handler, button) = await FindButtonHandlerAsync(request.ListContext.CollectionAlias, request.ActionId);

            var context = new ButtonContext(request.ListContext.Parent, request.CustomData);

            await handler.ButtonClickAfterRepositoryActionAsync(button, request.ListContext.ProtoEditContext, context);
        }

        private async Task<(IButtonActionHandler handler, IButtonSetup button)> FindButtonHandlerAsync(string collectionAlias, string buttonId)
        {
            var collection = await _collectionResolver.ResolveSetupAsync(collectionAlias);

            var button = collection.FindButton(buttonId);
            if (button == null)
            {
                throw new Exception($"Cannot determine which button triggered action for collection {collectionAlias}");
            }

            return (_buttonActionHandlerResolver.GetButtonActionHandler(button), button);
        }
    }
}
