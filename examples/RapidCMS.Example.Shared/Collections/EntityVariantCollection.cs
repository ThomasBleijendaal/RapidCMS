using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Repositories;
using RapidCMS.Example.Shared.Data;

namespace RapidCMS.Example.Shared.Collections
{
    public static class EntityVariantCollection
    {
        // CRUD editor with support for
        public static void AddEntityVariantCollection(this ICmsConfig config)
        {
            config.AddCollection<EntityVariantBase, BaseRepository<EntityVariantBase>>("variants", "ProductVariant", "OrangeYellow20", "Entity Variants", collection =>
            {
                collection
                    // Set showEntities to true to have this collection to fold open on default
                    .SetTreeView(x => x.Name, showEntitiesOnStartup: true)
                    .AddEntityVariant<EntityVariantA>("Variant A", "a")
                    .AddEntityVariant<EntityVariantB>("Variant B", "b")
                    .AddEntityVariant<EntityVariantC>("Variant C", "c")
                    .SetListEditor(view =>
                    {
                        view.AddDefaultButton(DefaultButtonType.New, label: "New {0}");
                        view.AddDefaultButton(DefaultButtonType.Return);

                        view.SetColumnVisibility(EmptyVariantColumnVisibility.Visible);

                        view.SetPageSize(10);

                        view
                            .AddSection(section =>
                            {
                                section.AddField(p => p.Id);

                                section.AddField(p => p.Name)
                                    .SetOrderByExpression(p => p.Name, OrderByType.Ascending);
                            })
                            .AddSection<EntityVariantA>(section =>
                            {
                                section.AddField(p => p.Id);

                                section.AddField(p => p.Name)
                                    .SetOrderByExpression(p => p.Name, OrderByType.Ascending);

                                section.AddField(p => p.NameA1);

                                section.AddDefaultButton(DefaultButtonType.SaveExisting);
                                section.AddDefaultButton(DefaultButtonType.SaveNew);
                                section.AddDefaultButton(DefaultButtonType.Edit);
                            });

                        view
                            .AddSection<EntityVariantB>(section =>
                            {
                                section.AddField(p => p.Id);

                                section.AddField(p => p.Name)
                                    .SetOrderByExpression(p => p.Name, OrderByType.Ascending);

                                section.AddField(p => p.NameB1);
                                section.AddField(p => p.NameB2);

                                section.AddDefaultButton(DefaultButtonType.SaveExisting);
                                section.AddDefaultButton(DefaultButtonType.SaveNew);
                                section.AddDefaultButton(DefaultButtonType.Edit);
                            });

                        view
                            .AddSection<EntityVariantC>(section =>
                            {
                                section.AddField(p => p.Id);

                                section.AddField(p => p.Name)
                                    .SetOrderByExpression(p => p.Name, OrderByType.Ascending);

                                section.AddField(p => p.NameC1);
                                section.AddField(p => p.NameC2);
                                section.AddField(p => p.NameC3);

                                section.AddDefaultButton(DefaultButtonType.SaveExisting);
                                section.AddDefaultButton(DefaultButtonType.SaveNew);
                                section.AddDefaultButton(DefaultButtonType.Edit);
                            });
                    })
                    .SetNodeEditor(editor =>
                    {
                        editor.AddDefaultButton(DefaultButtonType.Up);
                        editor.AddDefaultButton(DefaultButtonType.SaveNew);
                        editor.AddDefaultButton(DefaultButtonType.SaveExisting);

                        editor.AddSection(generic =>
                        {
                            generic.SetLabel("Generics");

                            generic.AddField(x => x.Id).SetType(DisplayType.Pre);
                            generic.AddField(x => x.Name);
                        });

                        editor.AddSection<EntityVariantA>(a =>
                        {
                            a.SetLabel("Variant A specifics");

                            a.AddField(x => x.NameA1);
                        });

                        editor.AddSection<EntityVariantB>(b =>
                        {
                            b.SetLabel("Variant B specifics");

                            b.AddField(x => x.NameB1);
                            b.AddField(x => x.NameB2);
                        });

                        editor.AddSection<EntityVariantC>(c =>
                        {
                            c.SetLabel("Variant C specifics");

                            c.AddField(x => x.NameC1);
                            c.AddField(x => x.NameC2);
                            c.AddField(x => x.NameC3);
                        });
                    });
            });
        }
    }
}
