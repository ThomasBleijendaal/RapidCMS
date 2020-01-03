using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Common.Forms;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.UI;

namespace RapidCMS.Core.Resolvers.UI
{
    public interface INodeUIResolver
    {
        Task<IEnumerable<ButtonUI>> GetButtonsForEditContextAsync(EditContext editContext);
        Task<IEnumerable<SectionUI>> GetSectionsForEditContextAsync(EditContext editContext);
    }
}
