using RapidCMS.Core.Forms;

namespace RapidCMS.UI.Models;

public class ButtonClickEventArgs
{
    public ButtonViewModel ViewModel { get; set; } = default!;
    public FormEditContext EditContext { get; set; } = default!;
    public object? Data { get; set; }
}
