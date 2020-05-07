using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Helpers;

namespace RapidCMS.Core.Models.Config.Convention
{
    internal class ConventionListViewConfig<TEntity> : ListConfig, IIsConventionBased
        where TEntity : IEntity
    {
        public ConventionListViewConfig(bool canGoToNodeEditor)
        {
            CanEdit = canGoToNodeEditor;
        }

        public bool CanEdit { get; }

        public T GenerateConfig<T>() where T : class
        {
            Expression<Func<IEntity, string?>> test = (IEntity x) => x.Id;

            if (typeof(T) != typeof(ListConfig))
            {
                throw new InvalidOperationException();
            }
            else
            {
                var result = new ListConfig
                {
                    PageSize = 25,
                    Buttons = new List<ButtonConfig>
                    {
                        new DefaultButtonConfig
                        {
                            ButtonType = DefaultButtonType.New
                        }
                    },
                    ListEditorType = ListType.Table,
                    Panes = new List<PaneConfig>
                    {
                        new PaneConfig<TEntity>(typeof(TEntity))
                        {
                            Buttons = new List<ButtonConfig>
                            {
                                new DefaultButtonConfig
                                {
                                    ButtonType = DefaultButtonType.Edit
                                },
                                new DefaultButtonConfig
                                {
                                    ButtonType = DefaultButtonType.Delete
                                }
                            },
                            FieldIndex = 1,
                            Fields = new List<FieldConfig>
                            {
                                new FieldConfig
                                {
                                    Description = "Test",
                                    Name = "Test",
                                    Property = PropertyMetadataHelper.GetPropertyMetadata(test)
                                }
                            },
                            VariantType = typeof(TEntity)
                        }
                    },
                    ReorderingAllowed = CanEdit,
                    SearchBarVisible = true
                };


                return (result as T)!;
            }
        }
    }
}
