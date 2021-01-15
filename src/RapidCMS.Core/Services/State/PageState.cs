using System.Collections.Generic;
using System.Linq;
using RapidCMS.Core.Abstractions.State;
using RapidCMS.Core.Models.State;

namespace RapidCMS.Core.Services.State
{
    internal class PageState : IPageState
    {
        private bool _updateState = false;

        private readonly List<PageStateModel> _currentState = new List<PageStateModel>();
        private readonly INavigationState _navigationState;

        public PageState(INavigationState navigationState)
        {
            _navigationState = navigationState;
        }

        public IEnumerable<PageStateModel> GetCurrentStates()
        {
            return _currentState;
        }

        public PageStateModel? GetCurrentState() => _currentState.LastOrDefault();

        public PageStateModel? PopState()
        {
            if (_currentState.Any())
            {
                _currentState.RemoveAt(_currentState.Count - 1);
            }

            NotifyUpdate();

            return _currentState.LastOrDefault();
        }

        public void PushState(PageStateModel newState)
        {
            _currentState.Add(newState);

            NotifyUpdate();
        }

        public void ReplaceState(PageStateModel replacementState)
        {
            if (_currentState.Any())
            {
                _currentState.RemoveAt(_currentState.Count - 1);
            }
            _currentState.Add(replacementState);

            NotifyUpdate();
        }

        public void ResetState(PageStateModel newState)
        {
            _currentState.Clear();
            _currentState.Add(newState);
        }

        public void UpdateNavigationStateWhenStateChanges()
        {
            _updateState = true;
        }

        private void NotifyUpdate()
        {
            if (_updateState && _currentState.Any())
            {
                _navigationState.NotifyLocationChanged(_currentState.Last());
            }
        }
    }
}
