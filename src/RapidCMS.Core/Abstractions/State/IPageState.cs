using System;
using System.Collections.Generic;
using RapidCMS.Core.Models.State;

namespace RapidCMS.Core.Abstractions.State
{
    [Obsolete]
    public interface IPageState
    {
        void UpdateNavigationStateWhenStateChanges();

        void PushState(PageStateModel newState);
        void ReplaceState(PageStateModel replacementState);
        PageStateModel? PopState();

        PageStateModel? GetCurrentState();
        IEnumerable<PageStateModel> GetCurrentStates();

        void ResetState(PageStateModel newState);
    }
}
