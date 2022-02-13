using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Abstractions.Navigation;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Navigation;

namespace RapidCMS.Core.Handlers
{
    public class NavigateButtonActionHandler<TNavigationHandler> : DefaultButtonActionHandler
        where TNavigationHandler : INavigationHandler
    {
        private readonly TNavigationHandler _navigationHandler;
        private readonly ISetupResolver<CollectionSetup> _collectionResolver;
        private readonly INavigationStateProvider _navigationStateProvider;

        public NavigateButtonActionHandler(
            TNavigationHandler navigationHandler,
            ISetupResolver<CollectionSetup> collectionResolver,
            INavigationStateProvider navigationStateProvider)
        {
            _navigationHandler = navigationHandler;
            _collectionResolver = collectionResolver;
            _navigationStateProvider = navigationStateProvider;
        }

        public override async Task<CrudType> ButtonClickBeforeRepositoryActionAsync(ButtonSetup button, FormEditContext editContext, ButtonContext context)
        {
            var request = await _navigationHandler.CreateNavigationRequestAsync(button, editContext);
            if (request != null)
            {
                var collection = request.IsPage ? null : await _collectionResolver.ResolveSetupAsync(request.CollectionAlias);

                var usageType = request.IsNew ? UsageType.New
                    : request.IsEdit ? UsageType.Edit
                    : UsageType.View;

                NavigationState navigationState;

                if (request.IsPage)
                {
                    navigationState = new NavigationState(request.CollectionAlias, usageType);
                }
                else if (request.IsList)
                {
                    navigationState = new NavigationState(request.CollectionAlias, request.ParentPath, usageType);
                }
                else
                {
                    navigationState = new NavigationState(request.CollectionAlias, request.ParentPath, request.VariantAlias ?? collection!.EntityVariant.Alias, request.Id, usageType);
                }

                _navigationStateProvider.AppendNavigationState(navigationState);
            }

            return CrudType.None;
        }
    }
}
