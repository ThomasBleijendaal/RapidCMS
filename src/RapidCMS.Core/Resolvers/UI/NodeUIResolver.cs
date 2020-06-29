using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RapidCMS.Core.Abstractions.Resolvers;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Models.UI;

namespace RapidCMS.Core.Resolvers.UI
{
    internal class NodeUIResolver : BaseUIResolver, INodeUIResolver
    {
        private readonly NodeSetup _node;

        public NodeUIResolver(
            NodeSetup node,
            IDataProviderResolver dataProviderService,
            IButtonActionHandlerResolver buttonActionHandlerResolver,
            IAuthService authService, 
            IHttpContextAccessor httpContextAccessor) : base(dataProviderService, buttonActionHandlerResolver, authService, httpContextAccessor)
        {
            _node = node;
        }

        public async Task<IEnumerable<ButtonUI>> GetButtonsForEditContextAsync(EditContext editContext)
        {
            if (_node.Buttons == null)
            {
                return Enumerable.Empty<ButtonUI>();
            }

            return await GetButtonsAsync(_node.Buttons, editContext).ConfigureAwait(false);
        }

        public async Task<IEnumerable<SectionUI>> GetSectionsForEditContextAsync(EditContext editContext)
        {
            if (_node.Panes == null)
            {
                return Enumerable.Empty<SectionUI>();
            }

            var type = editContext.Entity.GetType();

            return await _node.Panes
                .Where(pane => pane.VariantType.IsSameTypeOrBaseTypeOf(type))
                .ToListAsync(pane => GetSectionUIAsync(pane, editContext))
                .ConfigureAwait(false);
        }
    }
}
