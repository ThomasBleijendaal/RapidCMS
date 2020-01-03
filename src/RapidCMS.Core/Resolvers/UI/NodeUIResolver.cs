using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Setup;
using RapidCMS.Core.Models.UI;
using RapidCMS.Core.Resolvers;
using RapidCMS.Core.Resolvers.UI;

namespace RapidCMS.Common.Resolvers.UI
{
    internal class NodeUIResolver : BaseUIResolver, INodeUIResolver
    {
        private readonly NodeSetup _node;

        public NodeUIResolver(
            NodeSetup node, 
            IDataProviderResolver dataProviderService, 
            IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor) : base(dataProviderService, authorizationService, httpContextAccessor)
        {
            _node = node;
        }

        public async Task<IEnumerable<ButtonUI>> GetButtonsForEditContextAsync(EditContext editContext)
        {
            if (_node.Buttons == null)
            {
                return Enumerable.Empty<ButtonUI>();
            }

            return await GetButtonsAsync(_node.Buttons, editContext);
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
                .ToListAsync(pane => GetSectionUIAsync(pane, editContext));
        }
    }
}
