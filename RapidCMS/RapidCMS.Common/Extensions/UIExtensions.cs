using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Forms;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.UI;
using RapidCMS.Common.ValueMappers;

namespace RapidCMS.Common.Extensions
{
    public static class UIExtensions
    {
        public static IEnumerable<IRelation> GetRelations(this SectionUI section)
        {
            return section.Elements
                .Select(element => element as PropertyFieldUI)
                .Where(field => field != null)
                .Where(field => field?.DataCollection is IRelationDataCollection)
                .Select(field =>
                {
                    var collection = field.DataCollection as IRelationDataCollection;
                    var relatedElements = collection.GetCurrentRelatedElements();

                    return new Data.Relation(
                        collection.GetRelatedEntityType(),
                        field.Property, 
                        relatedElements.Select(x => new RelatedElement { Id = x.Id }));;
                });
        }

        internal static ButtonUI ToUI(this Button button)
        {
            return new ButtonUI
            {
                Icon = button.Icon,
                ButtonId = button.ButtonId,
                Label = button.Label,
                ShouldConfirm = button.ShouldConfirm,
                IsPrimary = button.IsPrimary,
                RequiresValidForm = button.RequiresValidForm,
                CustomAlias = (button is CustomButton customButton) ? customButton.Alias : null
            };
        }

        internal static FieldUI ToUI(this Field field)
        {
            return new FieldUI
            {
                Description = field.Description,
                Name = field.Name,
                Type = field.DataType,
                IsVisible = field.IsVisible
            };
        }

        internal static FieldUI ToUI(this Field field, IServiceProvider serviceProvider, DataContext dataContext)
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
                PropertyFieldUI ui;

                if (field is CustomField customPropertyField)
                {
                    ui = new CustomPropertyFieldUI
                    {
                        CustomAlias = customPropertyField.Alias
                    };
                }
                else
                {
                    ui = new PropertyFieldUI();
                }

                PopulateProperties(ui, propertyField, serviceProvider, dataContext);

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
        }

        private static void PopulateProperties(PropertyFieldUI ui, PropertyField field, IServiceProvider serviceProvider, DataContext dataContext)
        {
            PopulateProperties((FieldUI)ui, (Field)field);

            ui.Property = field.Property;
            ui.ValueMapper = field.ValueMapperType != null ? serviceProvider.GetService<IValueMapper>(field.ValueMapperType) : null;
            
            if (field.Relation != null)
            {
                ui.DataCollection = dataContext.GetDataCollection(field.Property);
            }
        }

        internal static SubCollectionUI ToUI(this SubCollectionList subCollection)
        {
            return new SubCollectionUI
            {
                CollectionAlias = subCollection.CollectionAlias,
                // TODO:
                IsVisible = x => true
            };
        }

        internal static RelatedCollectionUI ToUI(this RelatedCollectionList relatedCollection)
        {
            return new RelatedCollectionUI
            {
                CollectionAlias = relatedCollection.CollectionAlias,
                // TODO:
                IsVisible = x => true
            };
        }
    }
}
