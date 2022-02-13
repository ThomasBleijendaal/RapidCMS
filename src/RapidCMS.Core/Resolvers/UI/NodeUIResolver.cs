using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Navigation;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Models.UI;
using RapidCMS.Core.Navigation;

namespace RapidCMS.Core.Resolvers.UI
{
    internal class NodeUIResolver : BaseUIResolver, INodeUIResolver
    {
        private readonly NodeSetup _node;
        private readonly INavigationStateProvider _navigationStateProvider;

        public NodeUIResolver(
            NodeSetup node,
            IDataProviderResolver dataProviderService,
            IButtonActionHandlerResolver buttonActionHandlerResolver,
            INavigationStateProvider navigationStateProvider,
            IAuthService authService) : base(dataProviderService, buttonActionHandlerResolver, authService, navigationStateProvider)
        {
            _node = node;
            _navigationStateProvider = navigationStateProvider;
        }

        public async Task<IEnumerable<ButtonUI>> GetButtonsForEditContextAsync(FormEditContext editContext)
        {
            if (_node.Buttons == null)
            {
                return Enumerable.Empty<ButtonUI>();
            }

            return await GetButtonsAsync(_node.Buttons, editContext);
        }

        public async Task<IEnumerable<SectionUI>> GetSectionsForEditContextAsync(FormEditContext editContext, NavigationState navigationState)
        {
            if (_node.Panes == null)
            {
                return Enumerable.Empty<SectionUI>();
            }

            var type = editContext.Entity.GetType();

            var panes = await _node.Panes
                .Where(pane => pane.VariantType.IsSameTypeOrBaseTypeOf(type))
                .ToListAsync(pane => GetSectionUIAsync(pane, editContext, navigationState));
            
            return panes;
        }
    }
}
