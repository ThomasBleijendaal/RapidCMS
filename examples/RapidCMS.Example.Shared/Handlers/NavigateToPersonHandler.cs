using System.Threading.Tasks;
using RapidCMS.Core.Abstractions.Handlers;
using RapidCMS.Core.Abstractions.Setup;
using RapidCMS.Core.Forms;
using RapidCMS.Core.Models.Request;

namespace RapidCMS.Example.Shared.Handlers
{
    public class NavigateToPersonHandler : INavigationHandler
    {
        public Task<NavigationRequest?> CreateNavigationRequestAsync(IButton button, FormEditContext editContext)
        {
            // this method can return a NavigationRequest to instruct the CMS to go to that page
            // if null is returned, the action is canceled.
            return Task.FromResult(NavigationRequest.NavigateToCreateNewEntity("person", default))!;
        }
    }
}
