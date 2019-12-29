using RapidCMS.Common.Enums;
using RapidCMS.Common.Models.Metadata;

namespace RapidCMS.Common.Models.UI
{
    public class FieldUI : ElementUI
    {
        public string? Name { get; internal set; }
        public string? Description { get; internal set; }

        internal IPropertyMetadata? OrderByExpression { get; set; }

        public bool IsSortable() => OrderByExpression != null;
        public OrderByType SortDescending { get; set; }
    }
}
