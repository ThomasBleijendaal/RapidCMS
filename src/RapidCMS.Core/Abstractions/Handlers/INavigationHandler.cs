using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Request;

namespace RapidCMS.Core.Abstractions.Handlers
{
    public interface INavigationHandler
    {
        Task<NavigationRequest?> CreateNavigationRequestAsync(IButton button, FormEditContext editContext);
    }
}
