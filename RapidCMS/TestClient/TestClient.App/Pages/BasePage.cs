using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Services;
using RapidCMS.Common.Models.DTOs;

namespace TestClient.App.Pages
{
    public class BasePage : ComponentBase
    {
        [Inject]
        private IUriHelper UriHelper { get; set; }

        protected void HandleViewCommand(ViewCommand command)
        {
            if (command == null)
            {
                return;
            }

            switch (command)
            {
                case NavigateCommand navigateCommand:

                    UriHelper.NavigateTo(navigateCommand.Uri);

                    break;
                default:
                    break;
            }
        }


    }
}
