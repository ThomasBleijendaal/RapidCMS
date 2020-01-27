using RapidCMS.Core.Abstractions.Metadata;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Models.Setup;

namespace RapidCMS.Core.Models.UI
{
    public class FieldUI : ElementUI
    {
        internal FieldUI(FieldSetup field) : base(field.IsVisible, field.IsDisabled)
        {
            Description = field.Description;
            Name = field.Name;
            Expression = field.Expression;
            Property = field.Property;
            OrderByExpression = field.OrderByExpression;
            SortDescending = field.DefaultOrder;
        }

        public string? Name { get; private set; }
        public string? Description { get; private set; }

        public bool IsSortable() => OrderByExpression != null;

        internal IPropertyMetadata? Property { get; private set; }
        internal IExpressionMetadata? Expression { get; private set; }
        internal IPropertyMetadata? OrderByExpression { get; private set; }
        public OrderByType SortDescending { get; set; }
    }
}
