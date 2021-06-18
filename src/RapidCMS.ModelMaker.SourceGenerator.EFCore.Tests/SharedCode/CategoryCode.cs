namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests.SharedCode
{
    public static class CategoryCode
    {
        public const string EntityWithBlog = @"using System.Collections.Generic;
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
";

        public const string EntityWithoutBlog = @"using System.ComponentModel.DataAnnotations;
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
    }
}
";

        public const string Collection = @"using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
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
                ""categories"",
                ""Tag"",
                ""RedOrange10"",
                ""Categories"",
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
                            row.AddField(x => x.Name).SetName(""Name"");
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
                            section.AddField(x => x.Name).SetType(typeof(RapidCMS.UI.Components.Editors.TextBoxEditor)).SetName(""Name"");
                        });
                    });
                });
        }
    }
}
";

        public const string EntityConfiguration = @"using Microsoft.EntityFrameworkCore;
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
";

        public const string Repository = @"using System;
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
                _dbContext.Categories.Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
        }
        
        public override async Task<IEnumerable<Category>> GetAllAsync(IParent? parent, IQuery<Category> query)
        {
            return await query.ApplyOrder(query.ApplyDataView(_dbContext.Categories))
                .Skip(query.Skip)
                .Take(query.Take)
                .ToListAsync();
        }
        
        public override async Task<Category?> GetByIdAsync(string id, IParent? parent)
        {
            if (int.TryParse(id, out var intId))
            {
                return await _dbContext.Categories.FirstOrDefaultAsync(x => x.Id == intId);
            }
            return default;
        }
        
        public override async Task<Category?> InsertAsync(IEditContext<Category> editContext)
        {
            var entry = _dbContext.Categories.Add(editContext.Entity);
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
";
    }
}
