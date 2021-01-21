using System;
using RapidCMS.Core.Abstractions.Mediators;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Models.EventArgs.Mediators
{
    public class PaneRequestEventArgs : IMediatorRequestEventArgs<CrudType>
    {
        public PaneRequestEventArgs(Type paneType, FormEditContext editContext, ButtonContext buttonContext)
        {
            PaneType = paneType;
            EditContext = editContext ?? throw new ArgumentNullException(nameof(editContext));
            ButtonContext = buttonContext ?? throw new ArgumentNullException(nameof(buttonContext));
        }

        public Guid RequestId { get; set; }

        public Type PaneType { get; set; }
        public FormEditContext EditContext { get; set; }
        public ButtonContext ButtonContext { get; set; }
    }
}
