using System;
using System.Collections.Generic;
using System.Linq;
using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.Metadata;
using RapidCMS.Common.Models.UI;
using RapidCMS.Common.Services;
using RapidCMS.Common.ValueMappers;

namespace RapidCMS.Common.Extensions
{
    internal static class UIExtensions
    {
        public static IEnumerable<IRelation> GetRelations(this SectionUI section)
        {
            return section.Elements
                .Select(element => element as FieldWithLabelUI)
                .Where(field => field != null)
                .Where(field => field?.DataCollection is IRelationDataCollection)
                .Select(field =>
                {
                    var collection = field.DataCollection as IRelationDataCollection;
                    var relatedElements = collection.GetCurrentRelatedElements();

                    return new Relation(
                        field.Property, 
                        relatedElements.Select(x => new RelatedElement { Id = x.Id }));
                });
        }

        public static ButtonUI ToUI(this Button button)
        {
            return new ButtonUI
            {
                Icon = button.Icon,
                ButtonId = button.ButtonId,
                Label = button.Label,
                ShouldConfirm = button.ShouldConfirm,
                IsPrimary = button.IsPrimary,
                CustomAlias = (button is CustomButton customButton) ? customButton.Alias : null
            };
        }

        public static FieldUI ToUI(this Field field, IServiceProvider serviceProvider)
        {
            return PopulateProperties(new FieldUI(), field, serviceProvider);
        }

        private static T PopulateProperties<T>(T ui, Field field, IServiceProvider serviceProvider)
            where T : FieldUI
        {
            ui.CustomAlias = (field is CustomField customField) ? customField.Alias : null;

            ui.Expression = field.Expression;
            ui.Property = field.Property;

            ui.ValueMapper = serviceProvider.GetService<IValueMapper>(field.ValueMapperType);
            ui.Type = field.Readonly ? EditorType.Readonly : field.DataType;

            if (field.OneToManyRelation != null)
            {
                switch (field.OneToManyRelation)
                {
                    case OneToManyCollectionRelation collectionRelation:

                        var cr = serviceProvider.GetService<Root>(typeof(Root));

                        var repo = cr.GetRepository(collectionRelation.CollectionAlias);
                        var provider = new CollectionDataProvider();

                        provider.SetElementMetadata(repo, collectionRelation.IdProperty, collectionRelation.DisplayProperty);

                        ui.DataCollection = provider;

                        break;

                    case OneToManyDataProviderRelation dataProviderRelation:

                        ui.DataCollection = serviceProvider.GetService<IDataCollection>(dataProviderRelation.DataCollectionType);
                        break;
                }
            }

            return ui;
        }

        public static FieldWithLabelUI ToFieldWithLabelUI(this Field field, IServiceProvider serviceProvider)
        {
            var ui = PopulateProperties(new FieldWithLabelUI(), field, serviceProvider);

            ui.Description = field.Description;
            ui.Name = field.Name;

            return ui;
        }

        public static SubCollectionUI ToUI(this SubCollectionListEditor subCollection)
        {
            return new SubCollectionUI
            {
                CollectionAlias = subCollection.CollectionAlias
            };
        }
    }
}
