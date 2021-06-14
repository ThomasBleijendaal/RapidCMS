using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Enums;
using RapidCMS.ModelMaker.Models.Entities;
using RapidCMS.ModelMaker.Repositories;

namespace RapidCMS.ModelMaker.Collections
{
    public static class ModelCollection
    {
        public static void AddModelCollection(this ICmsConfig cmsConfig)
        {
            cmsConfig.AddCollection<ModelEntity, ModelRepository>(
                Constants.ModelMakerAdminCollectionAlias,
                "Database",
                "MagentaPink10",
                "Models",

                config =>
                {
                    config.SetTreeView(x => x.Name);

                    config.SetListView(view =>
                    {
                        view.AddDefaultButton(DefaultButtonType.New);

                        view.AddRow(row =>
                        {
                            row.AddField(x => x.Name);

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
                            section.AddField(x => x.Alias)
                                .SetName("Collection alias")
                                .SetType(EditorType.Readonly)
                                .VisibleWhen((m, s) => s == EntityState.IsExisting);

                            section.AddSubCollectionList("modelmaker::property");
                        });
                    });
                });
        }


    }
}
