using RapidCMS.Common.Attributes;

namespace RapidCMS.Common.Enums
{
    public enum DefaultButtonType
    {
        [DefaultIconLabel(Icon = "plus", Label = "New")]
        New = 1,

        [DefaultIconLabel(Icon = "save", Label = "Save new")]
        SaveNew = 2,

        [DefaultIconLabel(Icon = "save", Label = "Update")]
        SaveExisting = 3,

        [DefaultIconLabel(Icon = "save", Label = "Save")]
        SaveNewAndExisting = 4,

        [DefaultIconLabel(Icon = "trash", Label = "Delete")]
        Delete = 5
    }
}
