using Microsoft.AspNetCore.Components;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Providers;
using RapidCMS.ModelMaker.Enums;
using RapidCMS.ModelMaker.Models.Entities;
using RapidCMS.ModelMaker.Repositories;

namespace RapidCMS.ModelMaker.Collections
{
    internal static class ModelCollection
    {
        public static void AddModelCollection(this ICmsConfig cmsConfig)
        {
            var config = cmsConfig.AddCollection<ModelEntity, ModelRepository>(
                    Constants.ModelMakerAdminCollectionAlias,
                    "Database",
                    "MagentaPink10",
                    "Models",
                    x => { });

            config.SetTreeView(x => x.Name);

            config.SetListView(view =>
            {
                view.AddDefaultButton(DefaultButtonType.New);

                view.AddRow(row =>
                {
                    row.AddField(x => x.Name);
                    row.AddField(x => x.PluralName).SetName("Plural name");

                    row.AddDefaultButton(DefaultButtonType.Edit);
                });
            });

            config.SetNodeEditor(editor =>
            {
                editor.AddSection(section =>
                {
                    section.AddDefaultButton(DefaultButtonType.Up);
                    section.AddDefaultButton(DefaultButtonType.SaveExisting);
                    section.AddDefaultButton(DefaultButtonType.SaveNew);
                    section.AddDefaultButton(DefaultButtonType.Delete);

                    section.AddField(x => x.Name);

                    section.AddField(x => x.PluralName)
                        .SetName("Plural name");

                    section.AddField(x => x.IconColor)
                        .SetDetails(new MarkupString("See Shared Colors at <a href=\"https://developer.microsoft.com/en-us/fluentui#/styles/web/colors/shared\">https://developer.microsoft.com/en-us/fluentui#/styles/web/colors/shared</a>."))
                        .SetType(EditorType.Dropdown)
                        .SetDataCollection<EnumDataProvider<Color>>();

                    section.AddField(x => x.Icon)
                        .SetDetails(new MarkupString("See Fabric Core icons at <a href=\"https://developer.microsoft.com/en-us/fluentui#/styles/web/icons#available-icons\">https://developer.microsoft.com/en-us/fluentui#/styles/web/icons#available-icons</a>."));

                    section.AddField(x => x.Alias)
                        .SetName("Collection alias")
                        .SetType(EditorType.Readonly)
                        .VisibleWhen((m, s) => s == EntityState.IsExisting);

                    section.AddField(x => x.Output)
                        .SetDescription("Code will be generated for these items")
                        .SetType(EditorType.EnumFlagPicker)
                        .SetDataCollection<EnumDataProvider<OutputItem>>();

                    section.AddSubCollectionList("modelmaker::property");
                });
            });
        }
    }
}
