using System;
using System.Threading.Tasks;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Forms;

namespace RapidCMS.Core.Models.EventArgs
{
    public sealed class PaneEventArgs
    {
        public PaneEventArgs(Type paneType, Task returnTask, FormEditContext editContext, ButtonContext buttonContext, CrudType? defaultCrudType)
        {
            PaneType = paneType ?? throw new ArgumentNullException(nameof(paneType));
            ReturnTask = returnTask ?? throw new ArgumentNullException(nameof(returnTask));
            EditContext = editContext ?? throw new ArgumentNullException(nameof(editContext));
            ButtonContext = buttonContext ?? throw new ArgumentNullException(nameof(buttonContext));
            DefaultCrudType = defaultCrudType;
        }

        public Type PaneType { get; set; }
        public Task ReturnTask { get; set; }
        public FormEditContext EditContext { get; set; }
        public ButtonContext ButtonContext { get; set; }
        public CrudType? DefaultCrudType { get; set; }
    }
}
