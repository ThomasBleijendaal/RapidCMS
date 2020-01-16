using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using RapidCMS.Core;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Exceptions;
using RapidCMS.Core.Models.Commands;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.Response;

namespace RapidCMS.UI.Components.Pages
{
    public abstract class BasePage : ComponentBase
    {
        private UpdateParameterCommand? _previousParameterCommand = null;

        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IExceptionService ExceptionService { get; set; } = default!;

        [Parameter] public string Action { get; set; } = default!;
        [Parameter] public string CollectionAlias { get; set; } = default!;
        [Parameter] public string VariantAlias { get; set; } = default!;
        [Parameter] public string? Path { get; set; } = default!;
        [Parameter] public string? Id { get; set; } = default!;

        [Obsolete("In everything except init")]
        protected ParentPath? GetParentPath()
        {
            return ParentPath.TryParse(Path);
        }

        protected async Task HandleViewCommandAsync(ViewCommandResponseModel viewCommand)
        {
            try
            {
                var command = viewCommand?.ViewCommand;

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
                }

                switch (command)
                {
                    case NavigateCommand navigateCommand:

                        if (NavigationManager.Uri == new Uri(new Uri(NavigationManager.BaseUri), navigateCommand.Uri).AbsoluteUri)
                        {
                            // escape from New
                            Action = Constants.Edit;

                            await LoadDataAsync();
                        }
                        else
                        {
                            NavigationManager.NavigateTo(navigateCommand.Uri);
                        }

                        break;

                    case UpdateParameterCommand parameterCommand:

                        _previousParameterCommand = new UpdateParameterCommand
                        {
                            Action = Action,
                            CollectionAlias = CollectionAlias,
                            Id = Id,
                            ParentPath = Path,
                            VariantAlias = VariantAlias
                        };

                        var data = new Dictionary<string, object>()
                        {
                            { nameof(Action), parameterCommand.Action },
                            { nameof(CollectionAlias), parameterCommand.CollectionAlias }
                        };

                        if (parameterCommand.VariantAlias != null)
                        {
                            data.Add(nameof(VariantAlias), parameterCommand.VariantAlias);
                        }

                        if (parameterCommand.Id != null)
                        {
                            data.Add(nameof(Id), parameterCommand.Id);
                        }

                        if (parameterCommand.ParentPath != null)
                        {
                            data.Add(nameof(Path), parameterCommand.ParentPath);
                        }

                        var update = ParameterView.FromDictionary(data);
                        await SetParametersAsync(update);

                        break;

                    case ReloadCommand reloadCommand:
                        await LoadDataAsync(reloadCommand.EntityId);

                        break;

                    case NoOperationCommand _:
                    default:

                        StateHasChanged();

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
            // meh
            if (ex is UnauthorizedAccessException)
            {
                NavigationManager.NavigateTo("/unauthorized");
            }
            else if (ex is InvalidEntityException)
            {
                // trigger validation since entity is invalid
                // EditContext.IsValid();
            }
            else
            {
                ExceptionService.StoreException(ex);

                NavigationManager.NavigateTo("/error");
            }
        }

        protected virtual Task LoadDataAsync(IEnumerable<string>? reloadEntityIds = null)
        {
            return Task.CompletedTask;
        }

        [Obsolete("In everything except init")]
        protected UsageType GetUsageType()
        {
            var type = Action switch
            {
                Constants.Edit => UsageType.Edit,
                Constants.New => UsageType.New,
                Constants.Add => UsageType.Add,
                Constants.View => UsageType.View,
                Constants.List => UsageType.List,
                Constants.Pick => UsageType.Pick,
                _ => (UsageType)0
            };

            if (Path == null)
            {
                type |= UsageType.Root;
            }
            else
            {
                type |= UsageType.NotRoot;
            }

            return type;
        }
    }
}
