using System;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Forms;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.UI;

namespace RapidCMS.Common.Extensions
{
    public static class UIExtensions
    {
        internal static ButtonUI ToUI(this Button button, EditContext editContext)
        {
            return new ButtonUI
            {
                Icon = button.Icon,
                ButtonId = button.ButtonId,
                Label = button.Label,
                ShouldConfirm = button.ShouldAskForConfirmation(editContext),
                IsPrimary = button.IsPrimary,
                RequiresValidForm = button.RequiresValidForm(editContext),
                CustomType = button.CustomType
            };
        }

        internal static FieldUI ToUI(this Field field)
        {
            return new FieldUI
            {
                Description = field.Description,
                Name = field.Name,
                Type = field.DataType,
                IsVisible = field.IsVisible,
                IsDisabled = field.IsDisabled
            };
        }

        internal static FieldUI ToUI(this Field field, DataProvider? dataProvider)
        {
            if (field is ExpressionField expressionField)
            {
                var ui = new ExpressionFieldUI
                {
                    Expression = expressionField.Expression,
                };

                PopulateProperties(ui, field);

                return ui;
            }
            else if (field is PropertyField propertyField)
            {
                var ui = (field is CustomField customPropertyField)
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

        private static void PopulateProperties(FieldUI ui, Field field)
        {
            ui.Description = field.Description;
            ui.Name = field.Name;
            ui.Type = field.Readonly ? EditorType.Readonly : field.DataType;
            ui.IsVisible = field.IsVisible;
            ui.IsDisabled = field.IsDisabled;
        }

        private static void PopulateProperties(PropertyFieldUI ui, PropertyField field, DataProvider? dataProvider)
        {
            PopulateProperties(ui, field);

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
