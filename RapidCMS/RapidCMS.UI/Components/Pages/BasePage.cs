using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using RapidCMS.Common;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Exceptions;
using RapidCMS.Common.Helpers;
using RapidCMS.Common.Models.Commands;
using RapidCMS.UI.Models;

namespace RapidCMS.UI.Components.Pages
{
    public abstract class BasePage : ComponentBase
    {
        private UpdateParameterCommand? _previousParameterCommand = null;

        [Inject] private IUriHelper UriHelper { get; set; }
        [Inject] private IExceptionHelper ExceptionHelper { get; set; }
        [Inject] private IJSRuntime JSRuntime { get; set; }

        [CascadingParameter(Name = "CustomSections")] protected CustomSectionContainer CustomSections { get; set; }

        [Parameter] protected string Action { get; set; }
        [Parameter] protected string CollectionAlias { get; set; }
        [Parameter] protected string VariantAlias { get; set; }
        [Parameter] protected string? ParentId { get; set; } = null;
        [Parameter] protected string? Id { get; set; } = null;

        protected async Task HandleViewCommandAsync(ViewCommand command)
        {
            try
            {
                if (command == null)
                {
                    return;
                }

                if (command is ReturnCommand)
                {
                    if (_previousParameterCommand != null)
                    {
                        command = _previousParameterCommand;
                    }
                    else
                    {
                        command = new NavigateBackCommand();
                    }
                }

                switch (command)
                {
                    case NavigateCommand navigateCommand:

                        UriHelper.NavigateTo(navigateCommand.Uri);

                        break;

                    case NavigateBackCommand navigateBackCommand:

                        // TODO: improve this
                        await JSRuntime.InvokeAsync<bool>("history.back");

                        break;

                    case UpdateParameterCommand parameterCommand:

                        _previousParameterCommand = new UpdateParameterCommand
                        {
                            Action = Action,
                            CollectionAlias = CollectionAlias,
                            Id = Id,
                            ParentId = ParentId,
                            VariantAlias = VariantAlias
                        };

                        var data = new Dictionary<string, object>()
                        {
                            { "Action", parameterCommand.Action },
                            { "CollectionAlias", parameterCommand.CollectionAlias }
                        };

                        if (parameterCommand.VariantAlias != null)
                        {
                            data.Add("VariantAlias", parameterCommand.VariantAlias);
                        }

                        if (parameterCommand.Id != null)
                        {
                            data.Add("Id", parameterCommand.Id);
                        }

                        if (parameterCommand.ParentId != null)
                        {
                            data.Add("ParentId", parameterCommand.ParentId);
                        }

                        var update = ParameterCollection.FromDictionary(data);
                        await SetParametersAsync(update);

                        break;

                    case ReloadCommand reloadCommand:
                        await LoadDataAsync(reloadCommand.EntityId);

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
                await LoadDataAsync();
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
                // trigger validation since entity is invalid
                // EditContext.IsValid();
            }
            else
            {
                ExceptionHelper.StoreException(ex);

                UriHelper.NavigateTo("/error");
            }
        }

        protected virtual Task LoadDataAsync(IEnumerable<string>? reloadEntityIds = null)
        {
            return Task.CompletedTask;
        }

        protected UsageType GetUsageType()
        {
            return Action switch
            {
                Constants.Edit => UsageType.Edit,
                Constants.New => UsageType.New,
                Constants.Add => UsageType.Add,
                Constants.View => UsageType.View,
                Constants.List => UsageType.List,
                Constants.Pick => UsageType.Pick,
                _ => (UsageType)0
            };
        }
    }
}
