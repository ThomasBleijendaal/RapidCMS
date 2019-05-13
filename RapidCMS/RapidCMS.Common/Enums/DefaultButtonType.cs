using System;
using RapidCMS.Common.Attributes;

namespace RapidCMS.Common.Enums
{
    // TODO: update Actions attributes
    public enum DefaultButtonType
    {
        [DefaultIconLabel(Icon = "plus", Label = "New")]
        [Actions(UsageType.List)]
        New = 1,

        [DefaultIconLabel(Icon = "hard-drive", Label = "Insert")]
        [Actions(UsageType.New)]
        SaveNew,

        [DefaultIconLabel(Icon = "hard-drive", Label = "Update")]
        [Actions(UsageType.Edit | UsageType.Node)]
        SaveExisting,

        [Obsolete("This is weird")]
        [DefaultIconLabel(Icon = "hard-drive", Label = "Save")]
        [Actions(UsageType.New | UsageType.Node, UsageType.Edit | UsageType.Node)]
        SaveNewAndExisting,

        [DefaultIconLabel(Icon = "trash", Label = "Delete")]
        [Actions(UsageType.Edit | UsageType.Node, UsageType.View | UsageType.Node)]
        Delete,

        [DefaultIconLabel(Icon = "pencil", Label = "Edit")]
        [Actions(UsageType.List, UsageType.Node | UsageType.Edit, UsageType.Node | UsageType.View)]
        Edit,

        [DefaultIconLabel(Icon = "magnifying-glass", Label = "View")]
        [Actions(UsageType.List, UsageType.Node | UsageType.Edit, UsageType.Node | UsageType.View)]
        View

        //[DefaultIconLabel(Icon = "plus", Label = "Add")]
        //[Actions(UsageType.List)]
        //Add
    }
}
