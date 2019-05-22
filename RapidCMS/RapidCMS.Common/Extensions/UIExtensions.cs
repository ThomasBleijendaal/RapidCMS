using RapidCMS.Common.Data;
using RapidCMS.Common.Enums;
using RapidCMS.Common.Models;
using RapidCMS.Common.Models.UI;
using RapidCMS.Common.Services;
using RapidCMS.Common.ValueMappers;

namespace RapidCMS.Common.Extensions
{
    internal static class UIExtensions
    {
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

        public static FieldUI ToUI(this Field field)
        {
            return PopulateProperties(new FieldUI(), field);
        }

        private static T PopulateProperties<T>(T ui, Field field)
            where T : FieldUI
        {
            ui.CustomAlias = (field is CustomField customField) ? customField.Alias : null;
            ui.Property = field.Property;
            ui.ValueMapper = ServiceLocator.Instance.GetService<IValueMapper>(field.ValueMapperType);
            ui.Type = field.Readonly ? EditorType.Readonly : field.DataType;

            if (field.OneToManyRelation != null)
            {
                switch (field.OneToManyRelation)
                {
                    case OneToManyCollectionRelation collectionRelation:

                        var repo = Root.GetRepository(collectionRelation.CollectionAlias);
                        ui.DataProvider = new CollectionDataProvider(repo, collectionRelation.IdProperty, collectionRelation.DisplayProperty);
                        break;

                    case OneToManyDataProviderRelation dataProviderRelation:

                        ui.DataProvider = ServiceLocator.Instance.GetService<IDataProvider>(dataProviderRelation.DataProviderType);
                        break;
                }
            }

            return ui;
        }

        public static FieldWithLabelUI ToFieldWithLabelUI(this Field field)
        {
            var ui = PopulateProperties(new FieldWithLabelUI(), field);

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
