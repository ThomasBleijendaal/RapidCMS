using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
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

        internal static FieldUI ToUI(this Field field, IServiceProvider serviceProvider)
        {
            if (field is ExpressionField expressionField)
            {
                var ui = new ExpressionFieldUI
                {
                    Expression = expressionField.Expression
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

                PopulateProperties(ui, propertyField, serviceProvider);

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
        }

        private static void PopulateProperties(PropertyFieldUI ui, PropertyField field, IServiceProvider serviceProvider)
        {
            PopulateProperties(ui, field);

            ui.Property = field.Property;
            ui.ValueMapper = field.ValueMapperType != null ? serviceProvider.GetService<IValueMapper>(field.ValueMapperType) : null;
            
            if (field.Relation != null)
            {
                switch (field.Relation)
                {
                    case CollectionRelation collectionRelation:

                        // TODO: horrible
                        var cr = serviceProvider.GetService<Root>(typeof(Root));
                        var repo = cr.GetRepository(collectionRelation.CollectionAlias);
                        var provider = new CollectionDataProvider();



                        provider.SetElementMetadata(repo, collectionRelation.RelatedEntityType, collectionRelation.RepositoryParentIdProperty, collectionRelation.IdProperty, collectionRelation.DisplayProperty);

                        ui.DataCollection = provider;

                        break;

                    case DataProviderRelation dataProviderRelation:

                        ui.DataCollection = serviceProvider.GetService<IDataCollection>(dataProviderRelation.DataCollectionType);
                        break;
                }
            }
        }

        internal static SubCollectionUI ToUI(this SubCollectionList subCollection)
        {
            return new SubCollectionUI
            {
                CollectionAlias = subCollection.CollectionAlias
            };
        }
    }
}
