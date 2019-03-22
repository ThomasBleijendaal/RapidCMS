using RapidCMS.Common.Attributes;

namespace RapidCMS.Common.Enums
{
    public enum EditorType
    {
        TextBox = 0,
        TextArea,

        Readonly,

        [DefaultType(typeof(int), typeof(long), typeof(uint), typeof(ulong))]
        Numeric,

        Dropdown,
        Select
    }
}
