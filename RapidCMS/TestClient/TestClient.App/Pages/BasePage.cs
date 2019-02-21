using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Services;
using RapidCMS.Common.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestClient.App.Pages
{
    public class BasePage : ComponentBase
    {
        [Inject]
        private IUriHelper UriHelper { get; set; }

        protected async Task HandleViewCommandAsync(ViewCommand command)
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

                case UpdateParameterCommand parameterCommand:

                    var data = new Dictionary<string, object>()
                    {
                            { "Action", parameterCommand.Action },
                            { "Alias", parameterCommand.Alias }
                    };

                    if (parameterCommand.Id.HasValue)
                    {
                        data.Add("Id", parameterCommand.Id);
                    }

                    if (parameterCommand.ParentId.HasValue)
                    {
                        data.Add("ParentId", parameterCommand.ParentId);
                    }

                    var update = ParameterCollection.FromDictionary(data);

                    await SetParametersAsync(update);

                    break;

                case ReloadCommand _:
                    await LoadDataAsync();

                    break;

                default:
                    break;
            }
        }

        protected virtual Task LoadDataAsync()
        {
            return Task.CompletedTask;
        }
    }
}
