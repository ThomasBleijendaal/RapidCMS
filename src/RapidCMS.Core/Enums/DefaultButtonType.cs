using RapidCMS.Core.Attributes;

namespace RapidCMS.Core.Enums;

public enum DefaultButtonType
{
    // Insert new Entity
    [DefaultIconLabel(icon: "Add", label: "New")]
    [Actions(UsageType.List | UsageType.View, UsageType.List | UsageType.Edit)]
    New = 1,

    [DefaultIconLabel(icon: "Save", label: "Insert")]
    [Actions(UsageType.New)]
    [ValidForm]
    SaveNew,

    [DefaultIconLabel(icon: "Save", label: "Update")]
    [Actions(UsageType.Edit | UsageType.Node, UsageType.Edit | UsageType.List)]
    [ValidForm]
    SaveExisting,

    [DefaultIconLabel(icon: "Delete", label: "Delete")]
    [Actions(UsageType.Edit | UsageType.Node, UsageType.View | UsageType.Node)]
    [Confirm]
    Delete,

    [DefaultIconLabel(icon: "Edit", label: "Edit")]
    [Actions(UsageType.List | UsageType.View, UsageType.List | UsageType.Add, UsageType.Node | UsageType.Edit, UsageType.Node | UsageType.View)]
    Edit,

    [DefaultIconLabel(icon: "View", label: "View")]
    [Actions(UsageType.List, UsageType.Node | UsageType.Edit, UsageType.Node | UsageType.View)]
    View,

    // Add existing Entity
    [DefaultIconLabel(icon: "Add", label: "Add")]
    [Actions(UsageType.List | UsageType.Edit, UsageType.List | UsageType.View)]
    Add,

    // Remove existing Entity
    [DefaultIconLabel(icon: "Remove", label: "Remove")]
    [Actions(UsageType.Node | UsageType.Edit, UsageType.Node | UsageType.View)]
    Remove,

    // Pick existing Entity to Add
    [DefaultIconLabel(icon: "Add", label: "Pick")]
    [Actions(UsageType.Node | UsageType.Pick)]
    Pick,

    // return from New
    [DefaultIconLabel(icon: "Back", label: "Return")]
    [Actions(UsageType.List | UsageType.Add, UsageType.List | UsageType.New, UsageType.List | UsageType.Pick)]
    Return,

    // move up in tree
    [DefaultIconLabel(icon: "Up", label: "Up")]
    [Actions(UsageType.List | UsageType.NotRoot, UsageType.Node)]
    Up,

    [DefaultIconLabel(icon: "OpenPane", label: "[open pane]")]
    [Actions(UsageType.Node, UsageType.List)]
    OpenPane = 9990,

    [DefaultIconLabel(icon: "NavigateForward", label: "[navigate]")]
    [Actions(UsageType.Node, UsageType.List)]
    Navigate = 9991
}
