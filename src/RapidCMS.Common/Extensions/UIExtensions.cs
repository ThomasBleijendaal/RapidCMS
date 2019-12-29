using System;
using RapidCMS.Common.Data;
using RapidCMS.Common.Forms;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.UI;

namespace RapidCMS.Common.Extensions
{
    public static class UIExtensions
    {
        internal static ButtonUI ToUI(this Button button, EditContext editContext)
        {
            return new ButtonUI(button.ButtonId)
            {
                Icon = button.Icon,
                Label = button.Label,
                ShouldConfirm = button.ShouldAskForConfirmation(editContext),
                IsPrimary = button.IsPrimary,
                RequiresValidForm = button.RequiresValidForm(editContext),
                CustomType = button.CustomType
            };
        }

        internal static FieldUI ToUI(this Field field)
        {
            var ui = new FieldUI();

            PopulateBaseProperties(ui, field);

            return ui;
        }

        internal static FieldUI ToUI(this Field field, DataProvider? dataProvider)
        {
            if (field is ExpressionField expressionField)
            {
                var ui = (field is CustomExpressionField customExpressionField)
                    ? new CustomExpressionFieldUI
                    {
                        CustomType = customExpressionField.CustomType
                    }
                    : new ExpressionFieldUI();

                PopulateProperties(ui, expressionField);

                return ui;
            }
            else if (field is PropertyField propertyField)
            {
                var ui = (field is CustomPropertyField customPropertyField)
                    ? new CustomPropertyFieldUI
                    {
                        CustomType = customPropertyField.CustomType
                    }
                    : new PropertyFieldUI();

                PopulateProperties(ui, propertyField, dataProvider);

                return ui;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private static void PopulateBaseProperties(FieldUI ui, Field field)
        {
            ui.Description = field.Description;
            ui.Name = field.Name;
            ui.IsVisible = field.IsVisible;
            ui.IsDisabled = field.IsDisabled;
            ui.OrderByExpression = field.OrderByExpression;
            ui.SortDescending = field.DefaultOrder;
        }

        private static void PopulateProperties(ExpressionFieldUI ui, ExpressionField field)
        {
            PopulateBaseProperties(ui, field);

            ui.Expression = field.Expression;
            ui.Type = field.DisplayType;
        }

        private static void PopulateProperties(PropertyFieldUI ui, PropertyField field, DataProvider? dataProvider)
        {
            PopulateBaseProperties(ui, field);

            ui.Type = field.EditorType;
            ui.Property = field.Property;
            ui.DataCollection = dataProvider?.Collection;
        }

        internal static SubCollectionUI ToUI(this SubCollectionList subCollection)
        {
            return new SubCollectionUI
            {
                CollectionAlias = subCollection.CollectionAlias,
                // TODO:
                IsVisible = (x, y) => true
            };
        }

        internal static RelatedCollectionUI ToUI(this RelatedCollectionList relatedCollection)
        {
            return new RelatedCollectionUI
            {
                CollectionAlias = relatedCollection.CollectionAlias,
                // TODO:
                IsVisible = (x, y) => true
            };
        }
    }
}
