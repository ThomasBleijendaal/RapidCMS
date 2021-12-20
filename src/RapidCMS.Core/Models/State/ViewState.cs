using System;
using RapidCMS.Core.Abstractions.State;

namespace RapidCMS.Core.Models.State
{
    [Obsolete]
    public class ViewState
    {
        public static ViewState Api => new ViewState();

        private ViewState()
        {
            PageState = default!;
        }

        public ViewState(IPageState pageState)
        {
            PageState = pageState ?? throw new ArgumentNullException(nameof(pageState));
        }

        public IPageState PageState { get; private set; }
    }
}
