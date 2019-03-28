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

        [Relation(RelationType.One)]
        Dropdown,

        [Relation(RelationType.One)]
        Select,

        [Relation(RelationType.One)]
        MultiSelect
    }
}
