namespace RapidCMS.UI.Components.Editors
{
    public abstract class BaseMultiplePickerEditor : BasePicker
    {
        protected override bool IsMultiple { get; set; } = true;
    }
}
