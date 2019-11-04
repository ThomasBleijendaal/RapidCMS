using RapidCMS.Common.Attributes;

namespace RapidCMS.Common.Enums
{
    public enum DefaultButtonType
    {
        // Insert new Entity
        [DefaultIconLabel(Icon = "add", Label = "New")]
        [Actions(UsageType.List)]
        New = 1,

        [DefaultIconLabel(Icon = "save", Label = "Insert")]
        [Actions(UsageType.New)]
        [ValidForm]
        SaveNew,

        [DefaultIconLabel(Icon = "save", Label = "Update")]
        [Actions(UsageType.Edit | UsageType.Node, UsageType.Edit | UsageType.List)]
        [ValidForm]
        SaveExisting,

        [DefaultIconLabel(Icon = "trash", Label = "Delete")]
        [Actions(UsageType.Edit | UsageType.Node, UsageType.View | UsageType.Node)]
        [Confirm]
        Delete,

        [DefaultIconLabel(Icon = "create", Label = "Edit")]
        [Actions(UsageType.List, UsageType.Node | UsageType.Edit, UsageType.Node | UsageType.View)]
        Edit,

        [DefaultIconLabel(Icon = "search", Label = "View")]
        [Actions(UsageType.List, UsageType.Node | UsageType.Edit, UsageType.Node | UsageType.View)]
        View,

        // Add existing Entity
        [DefaultIconLabel(Icon = "add", Label = "Add")]
        [Actions(UsageType.List)]
        Add,

        // Remove existing Entity
        [DefaultIconLabel(Icon = "close", Label = "Remove")]
        [Actions(UsageType.Node | UsageType.Edit, UsageType.Node | UsageType.View)]
        Remove,

        // Pick existing Entity to Add
        [DefaultIconLabel(Icon = "add", Label = "Pick")]
        [Actions(UsageType.Node | UsageType.Pick)]
        Pick,

        [DefaultIconLabel(Icon = "arrow-back", Label = "Return")]
        [Actions(UsageType.Node, UsageType.List | UsageType.Add, UsageType.List | UsageType.New, UsageType.List | UsageType.Pick)]
        Return,

        [DefaultIconLabel(Icon = "open", Label = "[open pane]")]
        [Actions(UsageType.Node, UsageType.List)]
        OpenPane = 9990
    }
}
