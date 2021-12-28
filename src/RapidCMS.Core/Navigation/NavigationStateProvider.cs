using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Abstractions.Navigation;
using RapidCMS.Core.Models.Data;
using RapidCMS.Core.Models.EventArgs.Mediators;
using RapidCMS.Core.Models.UI;

namespace RapidCMS.Core.Navigation
{
    internal class NavigationStateProvider : INavigationStateProvider, IDisposable
    {
        private readonly IMediator _mediator;
        private readonly NavigationManager _navigationManager;

        // TODO: if a list of states in the NestedStates of each NavigationStates is kept, then it would be possible to update the url also for nested collection navigation events
        // this will restore the navigation state of collections via urls completely, but will be a bit hard when parsing the urls
        // (and experimental -- perhaps hide it behind Advanced flag)
        private List<NavigationState> _states = new();

        public NavigationStateProvider(
            IMediator mediator,
            NavigationManager navigationManager)
        {
            _mediator = mediator;
            _navigationManager = navigationManager;

            _navigationManager.LocationChanged += LocationChanged;
        }

        private void LocationChanged(object? sender, LocationChangedEventArgs e)
        {
            var uri = new Uri(e.Location);
            var currentState = GetCurrentState();
            var locationState = new NavigationState(uri.LocalPath, uri.Query);

            if (!currentState.Equals(locationState))
            {
                var newState = Initialize(uri.LocalPath, uri.Query);

                Notify(default, newState);
            }
        }

        public void AppendNavigationState(NavigationState newState)
        {
            var clonedState = (NavigationState)newState.Clone();

            _states.Add(clonedState);

            Notify(default, newState);
            UpdateUrl(newState);
        }


        public void AppendNavigationState(NavigationState currentState, NavigationState newState)
        {
            var clonedState = (NavigationState)newState.Clone();

            // top level state 
            if (_states.Contains(currentState))
            {
                _states.Add(clonedState);

                Notify(currentState, clonedState);
                UpdateUrl(clonedState);
            }
            else
            {
                var (parentState, foundState) = FindState(currentState);

                if (parentState == null || foundState == null)
                {
                    return;
                }

                ReplaceState(parentState.NestedStates, foundState, clonedState);

                Notify(foundState, clonedState);
            }
        }

        public void NestNavigationState(NavigationState currentState, NavigationState nestedState)
        {
            var (_, stateToAddNestedStateTo) = FindState(currentState);

            stateToAddNestedStateTo?.NestedStates.Add(nestedState);
        }

        public NavigationState GetCurrentState()
        {
            if (_states.Count == 0)
            {
                _states.Add(new NavigationState());
            }

            return _states.Last();
        }

        public NavigationState Initialize(string url, string queryString)
        {
            var newState = new NavigationState(url, queryString);

            _states.Clear();

            _states.Add(newState);

            return newState;
        }

        public bool RemoveNavigationState(NavigationState currentState)
        {
            var hasRemovedState = RemoveLastState(currentState);
            if (!hasRemovedState)
            {
                return false;
            } 
 
            var newState = GetCurrentState();

            Notify(currentState, newState);
            UpdateUrl(newState);

            return true;
        }

        public void ReplaceNavigationState(NavigationState currentState, NavigationState state)
        {
            var clonedState = (NavigationState)state.Clone();

            // top level state 
            if (_states.Contains(currentState))
            {
                ReplaceState(_states, currentState, clonedState);

                Notify(currentState, clonedState);
                UpdateUrl(clonedState);
            }
            else
            {
                var (parentState, foundState) = FindState(currentState);

                if (parentState == null || foundState == null)
                {
                    return;
                }

                ReplaceState(parentState.NestedStates, foundState, clonedState);

                Notify(foundState, clonedState);
            }
        }

        public void UpdateCollectionState(NavigationState currentState, CollectionState collectionState)
        {
            var (parentState, foundState) = FindState(currentState);

            if (foundState == null)
            {
                return;
            }

            foundState.CollectionState = collectionState;

            Notify(currentState, foundState);

            // if the found state is root state, then update url
            if (_states.Contains(foundState))
            {
                UpdateUrl(foundState);
            }
        }

        public IView GetCurrentView(NavigationState currentState, ListUI list)
        {
            var (_, state) = FindState(currentState);

            state ??= currentState;

            var view = View.Create(
                list.PageSize,
                state.CollectionState.CurrentPage,
                state.CollectionState.SearchTerm,
                state.CollectionState.ActiveTab,
                state.CollectionAlias);

            view.SetOrderBys(list.OrderBys);

            return view;
        }

        public bool TryProcessView(NavigationState currentState, IView view, bool hasSections)
        {
            var state = currentState.CollectionState;

            var overTheEnd = !view.MoreDataAvailable && state.CurrentPage > 1 && !hasSections;
            var atEnd = !view.MoreDataAvailable && !overTheEnd;
            var notAtEnd = view.MoreDataAvailable && state.MaxPage == state.CurrentPage;

            currentState.CollectionState = state with
            {
                MaxPage = notAtEnd || overTheEnd ? null : atEnd ? state.CurrentPage : state.MaxPage,
                CurrentPage = overTheEnd ? state.CurrentPage - 1 : state.CurrentPage
            };

            return !overTheEnd;
        }

        private void ReplaceState(IList<NavigationState> currentStates, NavigationState currentState, NavigationState newState)
        {
            var index = currentStates.IndexOf(currentState);
            if (index == -1)
            {
                return;
            }

            currentStates[index] = newState;
        }

        private bool RemoveLastState(NavigationState currentState)
        {
            if (_states.Contains(currentState))
            {
                var itemToRemove = _states.LastOrDefault();

                if (itemToRemove != null)
                {
                    _states.Remove(itemToRemove);
                    return true;
                }
            }

            return false;
        }

        private void Notify(NavigationState? oldState, NavigationState newState)
        {
            _mediator.NotifyEvent(this, new NavigationEventArgs(oldState, newState));
        }

        private void UpdateUrl(NavigationState state)
        {
            _navigationManager.NavigateTo(state.ToString());
        }

        private (NavigationState? parentState, NavigationState? foundState) FindState(NavigationState stateToFind)
        {
            if (_states.FirstOrDefault(x => x.Equals(stateToFind)) is NavigationState foundState)
            {
                return (default, foundState);
            }
            else
            {
                return _states.Select(x => FindState(x, stateToFind)).Where(x => x.foundState != null).FirstOrDefault();
            }
        }

        private (NavigationState? parentState, NavigationState? foundState) FindState(NavigationState parentState, NavigationState stateToFind)
        {
            if (parentState.NestedStates.FirstOrDefault(x => x.Equals(stateToFind)) is NavigationState foundState)
            {
                return (parentState, foundState);
            }
            else
            {
                return parentState.NestedStates.Select(x => FindState(x, stateToFind)).Where(x => x.foundState != null).FirstOrDefault();
            }
        }

        public void Dispose()
        {
            _navigationManager.LocationChanged -= LocationChanged;
        }
    }
}
