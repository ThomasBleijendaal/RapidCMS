using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using RapidCMS.Common.Extensions;
using RapidCMS.Common.Forms;
using RapidCMS.Common.Services;

namespace RapidCMS.Common.Models.UI
{
    public class NodeUI : BaseUI
    {
        private readonly Node _node;

        public NodeUI(Node node, IDataProviderService dataProviderService, IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor) : base(dataProviderService, authorizationService, httpContextAccessor)
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
