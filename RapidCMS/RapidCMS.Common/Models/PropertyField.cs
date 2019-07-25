using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Models
{
    internal class PropertyField : Field
    {
        internal IPropertyMetadata Property { get; set; }

        internal Relation? Relation { get; set; }
    }
}
