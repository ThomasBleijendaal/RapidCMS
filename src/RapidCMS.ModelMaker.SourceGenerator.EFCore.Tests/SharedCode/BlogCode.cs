﻿namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests.SharedCode
{
    public static class BlogCode
    {
        public const string Entity = @"using System.Collections.Generic;
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
";

        public const string Collection = @"using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
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
                ""blogs"",
                ""Blog"",
                ""Cyan30"",
                ""Blogs"",
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
                            row.AddField(x => x.Title).SetName(""Title"");
                            row.AddField(x => x.Content == null ? """" : x.Content.ToString().Truncate(25)).SetName(""Content"");
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
                            section.AddField(x => x.Title).SetType(typeof(RapidCMS.UI.Components.Editors.TextBoxEditor)).SetName(""Title"");
                            section.AddField(x => x.Content).SetType(typeof(RapidCMS.UI.Components.Editors.TextAreaEditor)).SetName(""Content"");
                            section.AddField(x => x.IsPublished).SetType(typeof(RapidCMS.UI.Components.Editors.DropdownEditor)).SetDataCollection(new RapidCMS.Core.Providers.FixedOptionsDataProvider(new (object, string)[] { (true, ""True""), (false, ""False"") })).SetName(""Is Published"");
                            section.AddField(x => x.PublishDate).SetType(typeof(RapidCMS.UI.Components.Editors.DateEditor)).SetName(""Publish Date"");
                            section.AddField(x => x.MainCategoryId).SetType(typeof(RapidCMS.UI.Components.Editors.EntityPicker)).SetCollectionRelation(""categories"").SetName(""Main Category"");
                            section.AddField(x => x.Categories).SetType(typeof(RapidCMS.UI.Components.Editors.EntitiesPicker)).SetCollectionRelation(""categories"").SetName(""Categories"");
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
    public class BlogConfiguration : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.HasOne(x => x.MainCategory).WithMany(x => x.BlogMainCategory).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Categories).WithMany(x => x.BlogCategories);
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
    public class BlogRepository : BaseRepository<Blog>
    {
        private readonly ModelMakerDbContext _dbContext;
        
        public BlogRepository(ModelMakerDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public override async Task DeleteAsync(string id, IParent? parent)
        {
            if (int.TryParse(id, out var intId))
            {
                var entity = await _dbContext.Blogs.Include(x => x.Categories).FirstOrDefaultAsync(x => x.Id == intId);
                if (entity != null)
                {
                    _dbContext.Blogs.Remove(entity);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }
        
        public override async Task<IEnumerable<Blog>> GetAllAsync(IParent? parent, IQuery<Blog> query)
        {
            return await query.ApplyOrder(query.ApplyDataView(_dbContext.Blogs)).Skip(query.Skip).Take(query.Take).AsNoTracking().ToListAsync();
        }
        
        public override async Task<Blog?> GetByIdAsync(string id, IParent? parent)
        {
            if (int.TryParse(id, out var intId))
            {
                return await _dbContext.Blogs.Include(x => x.Categories).AsNoTracking().FirstOrDefaultAsync(x => x.Id == intId);
            }
            return default;
        }
        
        public override async Task<Blog?> InsertAsync(IEditContext<Blog> editContext)
        {
            var entity = editContext.Entity;
            
            var relations = editContext.GetRelationContainer();
            await HandleCategoriesAsync(entity, relations);
            
            var entry = _dbContext.Blogs.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entry.Entity;
        }
        
        public override Task<Blog> NewAsync(IParent? parent, Type? variantType = null)
        {
            return Task.FromResult(new Blog());
        }
        
        public override async Task UpdateAsync(IEditContext<Blog> editContext)
        {
            var entity = await _dbContext.Blogs.Include(x => x.Categories).FirstAsync(x => x.Id == editContext.Entity.Id);
            
            entity.Content = editContext.Entity.Content;
            entity.IsPublished = editContext.Entity.IsPublished;
            entity.MainCategoryId = editContext.Entity.MainCategoryId;
            entity.PublishDate = editContext.Entity.PublishDate;
            entity.Title = editContext.Entity.Title;
            
            var relations = editContext.GetRelationContainer();
            await HandleCategoriesAsync(entity, relations);
            
            await _dbContext.SaveChangesAsync();
        }
        
        private async Task HandleCategoriesAsync(Blog dbEntity, IRelationContainer relations)
        {
            var selectedIds = relations.GetRelatedElementIdsFor<Blog, ICollection<Category>, int>(x => x.Categories) ?? Enumerable.Empty<int>();
            var existingIds = dbEntity.Categories.Select(x => x.Id);

            var itemsToRemove = dbEntity.Categories.Where(x => !selectedIds.Contains(x.Id)).ToList();
            var idsToAdd = selectedIds.Except(existingIds).ToList();

            var itemsToAdd = await _dbContext.Categories.Where(x => idsToAdd.Contains(x.Id)).ToListAsync();

            foreach (var itemToRemove in itemsToRemove)
            {
                dbEntity.Categories.Remove(itemToRemove);
            }
            foreach (var itemToAdd in itemsToAdd)
            {
                dbEntity.Categories.Add(itemToAdd);
            }
        }
    }
}
";
    }
}
