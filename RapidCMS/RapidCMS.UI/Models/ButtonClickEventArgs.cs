using RapidCMS.Common.Forms;

namespace RapidCMS.UI.Models
{
    public class ButtonClickEventArgs
    {
        public EditContext EditContext { get; set; }
        public object? Data { get; set; }
    }
}
