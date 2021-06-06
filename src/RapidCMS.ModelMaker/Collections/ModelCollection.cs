using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Enums;
using RapidCMS.ModelMaker.Components.Displays;
using RapidCMS.ModelMaker.Components.Sections;
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

                            row.AddField(x => x.State.ToString()).SetName("").SetType(typeof(PublishStateDisplay));

                            row.AddDefaultButton(DefaultButtonType.Edit);
                        });
                    });

                    config.SetNodeEditor(editor =>
                    {
                        editor.AddSection(typeof(ModelDetailsSection));

                        editor.AddSection(section =>
                        {
                            section.AddDefaultButton(DefaultButtonType.Up);
                            section.AddDefaultButton(DefaultButtonType.SaveExisting, "Publish changed");
                            section.AddDefaultButton(DefaultButtonType.SaveNew);
                            section.AddDefaultButton(DefaultButtonType.Delete);
                            // TODO: implement revert + custom button handler
                            // editor.AddDefaultButton(DefaultButtonType.SaveExisting, "Revert changes");

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
