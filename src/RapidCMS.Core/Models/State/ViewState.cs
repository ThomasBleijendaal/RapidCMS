using System;
using RapidCMS.Core.Abstractions.State;

namespace RapidCMS.Core.Models.State
{
    public class ViewState
    {
        public ViewState(IPageState pageState)
        {
            PageState = pageState ?? throw new ArgumentNullException(nameof(pageState));
        }

        public IPageState PageState { get; private set; }
    }
}
