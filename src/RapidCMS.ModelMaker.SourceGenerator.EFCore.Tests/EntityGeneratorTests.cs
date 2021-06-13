using NUnit.Framework;
using RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests.Helpers;

namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests
{
    public class EntityGeneratorTests
    {
        [Test]
        public void WhenBasicBlogConfigurationIsRead_ThenBlogEntitiesAreGenerated()
        {
            GeneratorTestHelper.TestGeneratedCode(new[] { "./modelmaker.blog.json", "./modelmaker.category.json" },
@"using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.ModelMaker
{
    public class Blog : IEntity
    {
        public int Id { get; set; }
        string? IEntity.Id { get => Id.ToString(); set => Id = int.Parse(value ?? ""0""); }
        
        [MinLength(1)]
        [MaxLength(127)]
        [RegularExpression(""[^a|b|c]"")]
        public System.String Title { get; set; }
        
        public System.String Content { get; set; }
        
        [Required]
        public System.Boolean IsPublished { get; set; }
        
        [Required]
        public System.DateTime PublishDate { get; set; }
        
        public ICollection<RapidCMS.ModelMaker.Category> Categories { get; set; } = new List<RapidCMS.ModelMaker.Category>();
    }
}
", 
@"using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Providers;
using RapidCMS.Core.Repositories;

namespace RapidCMS.ModelMaker
{
    public static class BlogCollection
    {
        public static void AddBlogCollection(this ICmsConfig config)
        {
            config.AddCollection<Blog, BaseRepository<Blog>>(
                ""blog"",
                ""Database"",
                ""Cyan30"",
                ""Blog"",
                collection =>
                {
                    collection.SetTreeView(x => x.Title);
                    collection.SetListView(view =>
                    {
                        view.AddDefaultButton(DefaultButtonType.New);
                        view.AddRow(row =>
                        {
                            row.AddField(x => x.Id.ToString()).SetName(""Id"");
                            row.AddField(x => x.Title);
                            row.AddDefaultButton(DefaultButtonType.Edit);
                            row.AddDefaultButton(DefaultButtonType.Delete);
                        });
                    });
                    collection.SetNodeEditor(editor =>
                    {
                        editor.AddDefaultButton(DefaultButtonType.Up);
                        editor.AddDefaultButton(DefaultButtonType.SaveExisting);
                        editor.AddDefaultButton(DefaultButtonType.SaveNew);
                        editor.AddSection(section =>
                        {
                            section.AddField(x => x.Title).SetType(typeof(RapidCMS.UI.Components.Editors.TextBoxEditor));
                            section.AddField(x => x.Content).SetType(typeof(RapidCMS.UI.Components.Editors.TextAreaEditor));
                            section.AddField(x => x.IsPublished).SetType(typeof(RapidCMS.UI.Components.Editors.DropdownEditor)).SetDataCollection(new RapidCMS.Core.Providers.FixedOptionsDataProvider(new (object, string)[] { (true, ""True""), (false, ""False"") }));
                            section.AddField(x => x.PublishDate).SetType(typeof(RapidCMS.UI.Components.Editors.DateEditor));
                            section.AddField(x => x.Categories).SetType(typeof(RapidCMS.UI.Components.Editors.EntitiesPickerEditor)).SetCollectionRelation(""category"");
                        });
                    });
                });
        }
    }
}
",
@"using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMS.ModelMaker
{
    public class Category : IEntity
    {
        public int Id { get; set; }
        string? IEntity.Id { get => Id.ToString(); set => Id = int.Parse(value ?? ""0""); }
        
        [Required]
        [MinLength(1)]
        [MaxLength(30)]
        public System.String Name { get; set; }

        public ICollection<RapidCMS.ModelMaker.Blog> Blog { get; set; } = new List<RapidCMS.ModelMaker.Blog>();
    }
}
",
@"using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Providers;
using RapidCMS.Core.Repositories;

namespace RapidCMS.ModelMaker
{
    public static class CategoryCollection
    {
        public static void AddCategoryCollection(this ICmsConfig config)
        {
            config.AddCollection<Category, BaseRepository<Category>>(
                ""category"",
                ""Database"",
                ""Cyan30"",
                ""Category"",
                collection =>
                {
                    collection.SetTreeView(x => x.Name);
                    collection.SetListView(view =>
                    {
                        view.AddDefaultButton(DefaultButtonType.New);
                        view.AddRow(row =>
                        {
                            row.AddField(x => x.Id.ToString()).SetName(""Id"");
                            row.AddField(x => x.Name);
                            row.AddDefaultButton(DefaultButtonType.Edit);
                            row.AddDefaultButton(DefaultButtonType.Delete);
                        });
                    });
                    collection.SetNodeEditor(editor =>
                    {
                        editor.AddDefaultButton(DefaultButtonType.Up);
                        editor.AddDefaultButton(DefaultButtonType.SaveExisting);
                        editor.AddDefaultButton(DefaultButtonType.SaveNew);
                        editor.AddSection(section =>
                        {
                            section.AddField(x => x.Name).SetType(typeof(RapidCMS.UI.Components.Editors.TextBoxEditor));
                        });
                    });
                });
        }
    }
}
",
@"using Microsoft.EntityFrameworkCore;

namespace RapidCMS.ModelMaker
{
    public class ModelMakerDbContext : DbContext
    {
        public ModelMakerDbContext(DbContextOptions options) : base(options)
        {
        }
        
        public DbSet<Blog> Blog { get; set; } = default!;
        
        public DbSet<Category> Category { get; set; } = default!;
    }
}
");
        }
    }
}
