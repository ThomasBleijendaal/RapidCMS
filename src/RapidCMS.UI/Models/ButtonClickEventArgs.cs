using RapidCMS.Core.Forms;

namespace RapidCMS.UI.Models
{
    public class ButtonClickEventArgs
    {
        public ButtonViewModel ViewModel { get; set; }
        public FormEditContext EditContext { get; set; }
        public object? Data { get; set; }
    }
}
