using RapidCMS.Common.Enums;


namespace RapidCMS.Common.Models
{
    internal class Field
    {
        internal int Index { get; set; }

        internal string Name { get; set; }
        internal string Description { get; set; }

        internal bool Readonly { get; set; } = true;

        internal EditorType DataType { get; set; } = EditorType.Readonly;
    }
}
