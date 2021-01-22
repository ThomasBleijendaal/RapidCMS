using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.EventArgs.Mediators;
using RapidCMS.Core.Models.State;

namespace RapidCMS.Core.Handlers
{
    internal class NavigateButtonActionHandler<TNavigationHandler> : DefaultButtonActionHandler
        where TNavigationHandler : INavigationHandler
    {
        private readonly TNavigationHandler _navigationHandler;
        private readonly ISetupResolver<ICollectionSetup> _collectionResolver;
        private readonly IMediator _mediator;

        public NavigateButtonActionHandler(
            TNavigationHandler navigationHandler,
            ISetupResolver<ICollectionSetup> collectionResolver,
            IMediator mediator)
        {
            _navigationHandler = navigationHandler;
            _collectionResolver = collectionResolver;
            _mediator = mediator;
        }

        public override async Task<CrudType> ButtonClickBeforeRepositoryActionAsync(IButton button, FormEditContext editContext, ButtonContext context)
        {
            var request = await _navigationHandler.CreateNavigationRequestAsync(button, editContext);
            if (request != null)
            {
                var collection = request.IsPage ? null : _collectionResolver.ResolveSetup(request.CollectionAlias);

                var pageType = request.IsPage ? PageType.Page 
                    : request.IsList ? PageType.Collection 
                    : PageType.Node;
                var usageType = request.IsNew ? UsageType.New
                    : request.IsEdit ? UsageType.Edit
                    : UsageType.View;

                _mediator.NotifyEvent(this, new NavigationEventArgs(new PageStateModel
                {
                    CollectionAlias = request.CollectionAlias,
                    Id = request.Id,
                    PageType = pageType,
                    ParentPath = request.ParentPath,
                    UsageType = usageType,
                    VariantAlias = request.VariantAlias ?? collection?.EntityVariant.Alias ?? string.Empty
                }, true));
            }

            return CrudType.None;
        }
    }
}
