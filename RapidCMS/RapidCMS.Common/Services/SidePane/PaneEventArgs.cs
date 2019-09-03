using System;
using System.Threading.Tasks;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Forms;

namespace RapidCMS.Common.Services.SidePane
{
    public class PaneEventArgs
    {
        public PaneEventArgs(Type paneType, Task returnTask, EditContext editContext, ButtonContext buttonContext, CrudType? defaultCrudType)
        {
            PaneType = paneType ?? throw new ArgumentNullException(nameof(paneType));
            ReturnTask = returnTask ?? throw new ArgumentNullException(nameof(returnTask));
            EditContext = editContext ?? throw new ArgumentNullException(nameof(editContext));
            ButtonContext = buttonContext ?? throw new ArgumentNullException(nameof(buttonContext));
            DefaultCrudType = defaultCrudType;
        }

        public Type PaneType { get; set; }
        public Task ReturnTask { get; set; }
        public EditContext EditContext { get; set; }
        public ButtonContext ButtonContext { get; set; }
        public CrudType? DefaultCrudType { get; set; }
    }
}
