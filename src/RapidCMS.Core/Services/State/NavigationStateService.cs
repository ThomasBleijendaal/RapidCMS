using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Primitives;
using RapidCMS.Core.Abstractions.Services;
using RapidCMS.Core.ChangeToken;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Helpers;
using RapidCMS.Core.Models.State;

namespace RapidCMS.Core.Services.State
{
    internal class NavigationState : INavigationState
    {
        protected internal CmsChangeToken _navigationChangeToken = new CmsChangeToken();
        private readonly List<NavigationStateModel> _currentState = new List<NavigationStateModel>();
        private readonly NavigationManager _navigationManager;

        public NavigationState(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        public IChangeToken ChangeToken => _navigationChangeToken;

        protected internal void NotifyUpdate()
        {
            var currentToken = _navigationChangeToken;
            _navigationChangeToken = new CmsChangeToken();
            currentToken.HasChanged = true;
        }

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

            NotifyUpdate();

            return _currentState.LastOrDefault();
        }

        public void PushState(NavigationStateModel newState)
        {
            _currentState.Add(newState);

            NotifyUpdate();
        }

        public void ReplaceState(NavigationStateModel replacementState)
        {
            if (_currentState.Any())
            {
                _currentState.Remove(_currentState.Last());
            }
            _currentState.Add(replacementState);

            NotifyUpdate();
        }

        public void ResetState(NavigationStateModel newState)
        {
            _currentState.Clear();
            _currentState.Add(newState);

            NotifyUpdate();
        }

        private void UpdateUrl()
        {
            var state = GetCurrentState();

            if (state != null)
            {
                var url = state.PageType switch
                {
                    PageType.Collection => UriHelper.Collection(
                        state.UsageType.HasFlag(UsageType.List) ? Constants.View : Constants.Edit,
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
