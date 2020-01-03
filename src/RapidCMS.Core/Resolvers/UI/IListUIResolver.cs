using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Common.Forms;
using RapidCMS.Common.Models.UI;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.UI;

namespace RapidCMS.Core.Resolvers.UI
{
    public interface IListUIResolver
    {
        ListUI GetListDetails();
        Task<IEnumerable<ButtonUI>> GetButtonsForEditContextAsync(EditContext editContext);
        Task<IEnumerable<SectionUI>> GetSectionsForEditContextAsync(EditContext editContext);
        Task<IEnumerable<TabUI>?> GetTabsAsync(EditContext editContext);
    }
}
