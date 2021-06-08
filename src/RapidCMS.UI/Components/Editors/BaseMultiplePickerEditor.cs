namespace RapidCMS.UI.Components.Editors
{
    public abstract class BaseMultiplePickerEditor : BasePickerEditor
    {
        protected override bool IsMultiple { get; set; } = true;
    }
}
