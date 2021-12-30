using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Dispatchers;
using RapidCMS.Core.Abstractions.Navigation;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Navigation;

namespace RapidCMS.Core.Dispatchers
{
    internal class GetPageDispatcher : IPresentationDispatcher<string, IEnumerable<ITypeRegistration>>
    {
        private readonly ISetupResolver<ICollectionSetup> _collectionResolver;
        private readonly IRepositoryResolver _repositoryResolver;
        private readonly IAuthService _authService;
        private readonly ILogin _loginRegistration;
        private readonly ISetupResolver<IPageSetup> _pageResolver;
        private readonly INavigationStateProvider _navigationStateProvider;

        public GetPageDispatcher(
            ISetupResolver<ICollectionSetup> collectionResolver,
            IRepositoryResolver repositoryResolver,
            IAuthService authService,
            ILogin loginRegistration,
            ISetupResolver<IPageSetup> pageResolver,
            INavigationStateProvider navigationStateProvider)
        {
            _collectionResolver = collectionResolver;
            _repositoryResolver = repositoryResolver;
            _authService = authService;
            _loginRegistration = loginRegistration;
            _pageResolver = pageResolver;
            _navigationStateProvider = navigationStateProvider;
        }

        public async Task<IEnumerable<ITypeRegistration>> GetAsync(string request)
        {
            var currentNavigationState = _navigationStateProvider.GetCurrentState();

            var configuredSections = (await _pageResolver.ResolveSetupAsync(request)).Sections;
            var sections = new List<ITypeRegistration>();
            var isAuthorized = default(bool?);

            foreach (var section in configuredSections)
            {
                if (section.Type == typeof(ICollectionConfig) && 
                    section.Parameters != null &&
                    section.Parameters.TryGetValue("InitialState", out var obj) &&
                    obj is NavigationState state)
                {
                    _navigationStateProvider.NestNavigationState(currentNavigationState, state);

                    var collection = await _collectionResolver.ResolveSetupAsync(state.CollectionAlias);
                    var repository = _repositoryResolver.GetRepository(collection);

                    var entity = await repository.NewAsync(new ViewContext(section.Alias, default), default);

                    if (await _authService.IsUserAuthorizedAsync(state.UsageType, entity))
                    {
                        isAuthorized = true;
                        sections.Add(section);
                    }
                    else
                    {
                        isAuthorized ??= false;
                    } 
                }
                else
                {
                    sections.Add(section);
                }
            }

            if (isAuthorized == false && 
                sections.Count == 0 &&
                (await _loginRegistration.CustomLandingPageRegistrationAsync()) is ITypeRegistration landingPage)
            {
                sections.Insert(0, landingPage);
            }

            return sections;
        }
    }
}
