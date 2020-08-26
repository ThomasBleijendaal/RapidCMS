using System.Collections.Generic;
using System.Threading.Tasks;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.UI;

namespace RapidCMS.Core.Abstractions.Resolvers
{
    public interface INodeUIResolver
    {
        Task<IEnumerable<ButtonUI>> GetButtonsForEditContextAsync(FormEditContext editContext);
        Task<IEnumerable<SectionUI>> GetSectionsForEditContextAsync(FormEditContext editContext);
    }
}
