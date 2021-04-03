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
                "modelmakeradmin",
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
                        editor.AddDefaultButton(DefaultButtonType.Up);
                        editor.AddDefaultButton(DefaultButtonType.SaveExisting, "Publish changed");
                        editor.AddDefaultButton(DefaultButtonType.SaveNew);
                        editor.AddDefaultButton(DefaultButtonType.Delete);
                        // TODO: implement revert + custom button handler
                        // editor.AddDefaultButton(DefaultButtonType.SaveExisting, "Revert changes");

                        editor.AddSection(section =>
                        {
                            section.AddField(x => x.Name);
                            section.AddField(x => x.Alias)
                                .SetName("Model alias")
                                .SetType(EditorType.Readonly)
                                .VisibleWhen((m, s) => s == EntityState.IsExisting);

                            section.AddSubCollectionList("modelmaker::property");
                        });
                    });
                });
        }


    }
}
