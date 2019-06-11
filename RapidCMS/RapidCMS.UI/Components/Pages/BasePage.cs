using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RapidCMS.Common.Exceptions;
using RapidCMS.Common.Helpers;
using RapidCMS.Common.Models.Commands;
using RapidCMS.Common.Models.UI;
using RapidCMS.Common.Validation;
using RapidCMS.UI.Components.Buttons;
using RapidCMS.UI.Models;


namespace RapidCMS.UI.Components.Pages
{
    public class BasePage : ComponentBase
    {
        [Inject]
        private IUriHelper UriHelper { get; set; }

        [Inject]
        private IExceptionHelper ExceptionHelper { get; set; }

        [CascadingParameter(Name = "CustomSections")]
        protected CustomSectionContainer CustomSections { get; set; }

        protected EditContext EditContext { get; private set; } 

        protected async Task HandleViewCommandAsync(ViewCommand command)
        {
            try
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
                            { "CollectionAlias", parameterCommand.CollectionAlias },
                            { "VariantAlias", parameterCommand.VariantAlias }
                        };

                        if (parameterCommand.Id != null)
                        {
                            data.Add("Id", parameterCommand.Id);
                        }

                        if (parameterCommand.ParentId != null)
                        {
                            data.Add("ParentId", parameterCommand.ParentId);
                        }

                        var update = ParameterCollection.FromDictionary(data);

                        // TODO: this sets invalid parameters..
                        await SetParametersAsync(update);

                        break;

                    case ReloadCommand _:
                        await LoadDataAsync();

                        break;

                    case NoOperationCommand _:
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            try
            {
                // TODO: manage when reloading
                // TODO: manage nesting of pages
                if (EditContext == null)
                {
                    EditContext = new EditContext();
                }

                await LoadDataAsync();

                EditContext.RequestValidation();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        protected void HandleException(Exception ex)
        {
            if (ex is UnauthorizedAccessException)
            {
                UriHelper.NavigateTo("/unauthorized");
            }
            else if (ex is InvalidEntityException)
            {
                // crash burn die
                throw ex;
            }
            else
            {
                ExceptionHelper.StoreException(ex);

                UriHelper.NavigateTo("/error");
            }
        }

        protected virtual Task LoadDataAsync()
        {
            return Task.CompletedTask;
        }

        protected ButtonContext<TContext> CreateButtonContext<TContext>(TContext context, ButtonUI button, Func<string, TContext, object?, Task> callback)
        {
            return new ButtonContext<TContext>(EditContext)
            {
                ButtonId = button.ButtonId,
                CallbackAsync = callback,
                Context = context,
                Icon = button.Icon,
                Label = button.Label,
                ShouldConfirm = button.ShouldConfirm,
                IsPrimary = button.IsPrimary
            };
        }
    }
}
