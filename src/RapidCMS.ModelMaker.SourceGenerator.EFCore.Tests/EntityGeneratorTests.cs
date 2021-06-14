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

#nullable enable

namespace RapidCMS.ModelMaker
{
    public partial class Blog : IEntity
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
        
        [Required]
        public int? MainCategoryId { get; set; }
        public RapidCMS.ModelMaker.Category? MainCategory { get; set; }
        
        public ICollection<RapidCMS.ModelMaker.Category> Categories { get; set; } = new List<RapidCMS.ModelMaker.Category>();
    }
}
",
@"using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Providers;
using RapidCMS.Core.Repositories;

#nullable enable

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
                    collection.SetElementConfiguration(x => x.Id, x => x.Title);
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
                            section.AddField(x => x.MainCategoryId).SetType(typeof(RapidCMS.UI.Components.Editors.EntityPickerEditor)).SetCollectionRelation(""category"");
                            section.AddField(x => x.Categories).SetType(typeof(RapidCMS.UI.Components.Editors.EntitiesPickerEditor)).SetCollectionRelation(""category"");
                        });
                    });
                });
        }
    }
}
",
@"using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RapidCMS.Core.Abstractions.Data;

#nullable enable

namespace RapidCMS.ModelMaker
{
    public partial class Category : IEntity
    {
        public int Id { get; set; }
        string? IEntity.Id { get => Id.ToString(); set => Id = int.Parse(value ?? ""0""); }
        
        [Required]
        [MinLength(1)]
        [MaxLength(30)]
        public System.String Name { get; set; }
        
        public ICollection<RapidCMS.ModelMaker.Blog> BlogMainCategory { get; set; } = new List<RapidCMS.ModelMaker.Blog>();
        
        public ICollection<RapidCMS.ModelMaker.Blog> BlogCategories { get; set; } = new List<RapidCMS.ModelMaker.Blog>();
    }
}
",
@"using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Providers;
using RapidCMS.Core.Repositories;

#nullable enable

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
                    collection.SetElementConfiguration(x => x.Id, x => x.Name);
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
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#nullable enable

namespace RapidCMS.ModelMaker
{
    public partial class ModelMakerDbContext : DbContext
    {
        public ModelMakerDbContext(DbContextOptions options) : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.ApplyConfiguration(new BlogConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        }
        
        public DbSet<Blog> Blog { get; set; } = default!;
        
        public DbSet<Category> Category { get; set; } = default!;
    }
}
",
@"using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#nullable enable

namespace RapidCMS.ModelMaker
{
    public class BlogConfiguration : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.HasOne(x => x.MainCategory).WithMany(x => x.BlogMainCategory).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Categories).WithMany(x => x.BlogCategories);
        }
    }
}
",
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Repositories;

#nullable enable

namespace RapidCMS.ModelMaker
{
    public class BlogRepository : BaseRepository<Blog>
    {
        private readonly ModelMakerDbContext _dbContext;
        
        public BlogRepository(ModelMakerDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public override async Task DeleteAsync(string id, IParent? parent)
        {
            var entity = await GetByIdAsync(id, parent);
            if (entity != null)
            {
                _dbContext.Blog.Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
        }
        
        public override async Task<IEnumerable<Blog>> GetAllAsync(IParent? parent, IQuery<Blog> query)
        {
            return await query.ApplyOrder(query.ApplyDataView(_dbContext.Blog))
                .Skip(query.Skip)
                .Take(query.Take)
                .ToListAsync();
        }
        
        public override async Task<Blog?> GetByIdAsync(string id, IParent? parent)
        {
            if (int.TryParse(id, out var intId))
            {
                return await _dbContext.Blog.FirstOrDefaultAsync(x => x.Id == intId);
            }
            return default;
        }
        
        public override async Task<Blog?> InsertAsync(IEditContext<Blog> editContext)
        {
            var entry = _dbContext.Blog.Add(editContext.Entity);
            await _dbContext.SaveChangesAsync();
            return entry.Entity;
        }
        
        public override Task<Blog> NewAsync(IParent? parent, Type? variantType = null)
        {
            return Task.FromResult(new Blog());
        }
        
        public override async Task UpdateAsync(IEditContext<Blog> editContext)
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
",
@"using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#nullable enable

namespace RapidCMS.ModelMaker
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
        }
    }
}
",
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Repositories;

#nullable enable

namespace RapidCMS.ModelMaker
{
    public class CategoryRepository : BaseRepository<Category>
    {
        private readonly ModelMakerDbContext _dbContext;
        
        public CategoryRepository(ModelMakerDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public override async Task DeleteAsync(string id, IParent? parent)
        {
            var entity = await GetByIdAsync(id, parent);
            if (entity != null)
            {
                _dbContext.Category.Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
        }
        
        public override async Task<IEnumerable<Category>> GetAllAsync(IParent? parent, IQuery<Category> query)
        {
            return await query.ApplyOrder(query.ApplyDataView(_dbContext.Category))
                .Skip(query.Skip)
                .Take(query.Take)
                .ToListAsync();
        }
        
        public override async Task<Category?> GetByIdAsync(string id, IParent? parent)
        {
            if (int.TryParse(id, out var intId))
            {
                return await _dbContext.Category.FirstOrDefaultAsync(x => x.Id == intId);
            }
            return default;
        }
        
        public override async Task<Category?> InsertAsync(IEditContext<Category> editContext)
        {
            var entry = _dbContext.Category.Add(editContext.Entity);
            await _dbContext.SaveChangesAsync();
            return entry.Entity;
        }
        
        public override Task<Category> NewAsync(IParent? parent, Type? variantType = null)
        {
            return Task.FromResult(new Category());
        }
        
        public override async Task UpdateAsync(IEditContext<Category> editContext)
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
");
        }
    }
}
