using RapidCMS.Common.Attributes;

namespace RapidCMS.Common.Enums
{
    // TODO: change actions to something configurable when actions become enums

    public enum DefaultButtonType
    {
        [DefaultIconLabel(Icon = "plus", Label = "New")]
        [Actions(Constants.List)]
        New = 1,

        [DefaultIconLabel(Icon = "hard-drive", Label = "Save new")]
        [Actions(Constants.New)]
        SaveNew,

        [DefaultIconLabel(Icon = "hard-drive", Label = "Update")]
        [Actions(Constants.Edit)]
        SaveExisting,

        [DefaultIconLabel(Icon = "hard-drive", Label = "Save")]
        [Actions(Constants.New, Constants.Edit)]
        SaveNewAndExisting ,

        [DefaultIconLabel(Icon = "trash", Label = "Delete")]
        [Actions(Constants.Edit)]
        Delete,

        [DefaultIconLabel(Icon = "pencil", Label = "Edit")]
        [Actions(Constants.List)]
        Edit,

        [DefaultIconLabel(Icon = "magnifying-glass", Label = "View")]
        [Actions(Constants.List)]
        View
    }
}
