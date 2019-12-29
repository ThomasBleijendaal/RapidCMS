using RapidCMS.Common.Attributes;

namespace RapidCMS.Common.Enums
{
    public enum DefaultButtonType
    {
        // Insert new Entity
        [DefaultIconLabel(icon: "add", label: "New")]
        [Actions(UsageType.List)]
        New = 1,

        [DefaultIconLabel(icon: "save", label: "Insert")]
        [Actions(UsageType.New)]
        [ValidForm]
        SaveNew,

        [DefaultIconLabel(icon: "save", label: "Update")]
        [Actions(UsageType.Edit | UsageType.Node, UsageType.Edit | UsageType.List)]
        [ValidForm]
        SaveExisting,

        [DefaultIconLabel(icon: "trash", label: "Delete")]
        [Actions(UsageType.Edit | UsageType.Node, UsageType.View | UsageType.Node)]
        [Confirm]
        Delete,

        [DefaultIconLabel(icon: "create", label: "Edit")]
        [Actions(UsageType.List, UsageType.Node | UsageType.Edit, UsageType.Node | UsageType.View)]
        Edit,

        [DefaultIconLabel(icon: "search", label: "View")]
        [Actions(UsageType.List, UsageType.Node | UsageType.Edit, UsageType.Node | UsageType.View)]
        View,

        // Add existing Entity
        [DefaultIconLabel(icon: "add", label: "Add")]
        [Actions(UsageType.List)]
        Add,

        // Remove existing Entity
        [DefaultIconLabel(icon: "close", label: "Remove")]
        [Actions(UsageType.Node | UsageType.Edit, UsageType.Node | UsageType.View)]
        Remove,

        // Pick existing Entity to Add
        [DefaultIconLabel(icon: "add", label: "Pick")]
        [Actions(UsageType.Node | UsageType.Pick)]
        Pick,

        // return from New
        [DefaultIconLabel(icon: "arrow-back", label: "Return")]
        [Actions(UsageType.List | UsageType.Add, UsageType.List | UsageType.New, UsageType.List | UsageType.Pick)]
        Return,

        // move up in tree
        [DefaultIconLabel(icon: "arrow-up", label: "Up")]
        [Actions(UsageType.List | UsageType.NotRoot, UsageType.Node)]
        Up,

        [DefaultIconLabel(icon: "open", label: "[open pane]")]
        [Actions(UsageType.Node, UsageType.List)]
        OpenPane = 9990
    }
}
