using System.Threading.Tasks;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Request;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Abstractions.Handlers;

public interface INavigationHandler
{
    Task<NavigationRequest?> CreateNavigationRequestAsync(ButtonSetup button, FormEditContext editContext);
}
