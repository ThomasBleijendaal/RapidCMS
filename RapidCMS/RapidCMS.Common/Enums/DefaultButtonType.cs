using RapidCMS.Common.Attributes;

namespace RapidCMS.Common.Enums
{
    public enum DefaultButtonType
    {
        [DefaultIconLabel(Icon = "plus", Label = "New")]
        [Actions(Constants.List)]
        New = 1,

        [DefaultIconLabel(Icon = "save", Label = "Save new")]
        [Actions(Constants.New)]
        SaveNew = 2,

        [DefaultIconLabel(Icon = "save", Label = "Update")]
        [Actions(Constants.Edit)]
        SaveExisting = 3,

        [DefaultIconLabel(Icon = "save", Label = "Save")]
        [Actions(Constants.New, Constants.Edit)]
        SaveNewAndExisting = 4,

        [DefaultIconLabel(Icon = "trash", Label = "Delete")]
        [Actions(Constants.Edit)]
        Delete = 5
    }
}
