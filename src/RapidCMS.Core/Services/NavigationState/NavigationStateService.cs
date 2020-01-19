using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Primitives;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.NavigationState;

namespace RapidCMS.Core.Services.NavigationState
{
    internal class NavigationStateService : INavigationStateService
    {
        private readonly List<NavigationStateModel> _currentState = new List<NavigationStateModel>();
        private readonly NavigationManager _navigationManager;

        public NavigationStateService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        public IChangeToken ChangeToken => throw new NotImplementedException();

        public IEnumerable<NavigationStateModel> GetCurrentStates()
        {
            return _currentState;
        }

        public NavigationStateModel? GetCurrentState() => _currentState.LastOrDefault();

        public NavigationStateModel? PopState()
        {
            if (_currentState.Any())
            {
                _currentState.Remove(_currentState.Last());
            }

            UpdateUrl();

            return _currentState.LastOrDefault();
        }

        public void PushState(NavigationStateModel newState)
        {
            _currentState.Add(newState);

            UpdateUrl();
        }

        public void ReplaceState(NavigationStateModel replacementState)
        {
            PopState();
            _currentState.Add(replacementState);

            UpdateUrl();
        }

        public void ResetState()
        {
            _currentState.Clear();
        }

        private void UpdateUrl()
        {
            var state = GetCurrentState();

            if (state != null)
            {
                var url = state.PageType switch
                {
                    PageType.Collection => UriHelper.Collection(
                        state.UsageType.HasFlag(UsageType.List) ? Constants.List : Constants.Edit,
                        state.CollectionAlias,
                        state.ParentPath),

                    PageType.Node => UriHelper.Node(
                        state.UsageType.HasFlag(UsageType.View) ? Constants.View : Constants.Edit,
                        state.CollectionAlias,
                        state.VariantAlias,
                        state.ParentPath,
                        state.Id),

                        _ => ""
                };

                // TODO: this triggers a new OnParameterSet which it should not
                // _navigationManager.NavigateTo(url, false);
            }
        }
    }
}
