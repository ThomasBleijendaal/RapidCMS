namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests.SharedCode
{
    public static class BlogCode
    {
        public const string Entity = @"using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Data;

#nullable enable

namespace RapidCMS.ModelMaker
{
    public partial class Blog : IEntity
    {
        public int Id { get; set; }
        string? IEntity.Id { get => Id.ToString(); set => Id = int.Parse(value ?? ""0""); }
        
        public System.String Title { get; set; }
        
        public System.String Content { get; set; }
        
        public System.Boolean IsPublished { get; set; }
        
        public System.DateTime PublishDate { get; set; }
        
        public int? MainCategoryId { get; set; }
        public RapidCMS.ModelMaker.Category? MainCategory { get; set; }
        
        public ICollection<RapidCMS.ModelMaker.Category> BlogCategories { get; set; } = new List<RapidCMS.ModelMaker.Category>();
    }
}
";

        public const string Collection = @"using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Providers;
using RapidCMS.Core.Repositories;
using RapidCMS.Example.ModelMaker.Validators;
using RapidCMS.ModelMaker.Validation.Config;

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
                    collection.AddEntityValidator<BlogValidator>();
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
                            section.AddField(x => x.IsPublished).SetType(typeof(RapidCMS.UI.Components.Editors.DropdownEditor)).SetDataCollection<RapidCMS.ModelMaker.DataCollections.BooleanLabelDataCollection, BooleanLabelDetailConfig>(new BooleanLabelDetailConfig { Labels = new BooleanLabelDetailConfig.LabelsConfig { TrueLabel = ""True"", FalseLabel = ""False"" } }).SetName(""Is Published"");
                            section.AddField(x => x.PublishDate).SetType(typeof(RapidCMS.UI.Components.Editors.DateEditor)).SetName(""Publish Date"");
                            section.AddField(x => x.MainCategoryId).SetType(typeof(RapidCMS.UI.Components.Editors.EntityPicker)).SetCollectionRelation(""categories"").SetName(""Main Category"");
                            section.AddField(x => x.BlogCategories).SetType(typeof(RapidCMS.UI.Components.Editors.EntitiesPicker)).SetCollectionRelation(""categories"").SetName(""Blog Categories"");
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
            builder.HasMany(x => x.BlogCategories).WithMany(x => x.BlogBlogCategories);
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
                var entity = await _dbContext.Blogs.Include(x => x.BlogCategories).FirstOrDefaultAsync(x => x.Id == intId);
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
                return await _dbContext.Blogs.Include(x => x.BlogCategories).AsNoTracking().FirstOrDefaultAsync(x => x.Id == intId);
            }
            return default;
        }
        
        public override async Task<Blog?> InsertAsync(IEditContext<Blog> editContext)
        {
            var entity = editContext.Entity;
            
            var relations = editContext.GetRelationContainer();
            await HandleBlogCategoriesAsync(entity, relations);
            
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
            var entity = await _dbContext.Blogs.Include(x => x.BlogCategories).FirstAsync(x => x.Id == editContext.Entity.Id);
            
            entity.Content = editContext.Entity.Content;
            entity.IsPublished = editContext.Entity.IsPublished;
            entity.MainCategoryId = editContext.Entity.MainCategoryId;
            entity.PublishDate = editContext.Entity.PublishDate;
            entity.Title = editContext.Entity.Title;
            
            var relations = editContext.GetRelationContainer();
            await HandleBlogCategoriesAsync(entity, relations);
            
            await _dbContext.SaveChangesAsync();
        }
        
        private async Task HandleBlogCategoriesAsync(Blog dbEntity, IRelationContainer relations)
        {
            var selectedIds = relations.GetRelatedElementIdsFor<Blog, ICollection<RapidCMS.ModelMaker.Category>, int>(x => x.BlogCategories) ?? Enumerable.Empty<int>();
            var existingIds = dbEntity.BlogCategories.Select(x => x.Id);
            
            var itemsToRemove = dbEntity.BlogCategories.Where(x => !selectedIds.Contains(x.Id)).ToList();
            var idsToAdd = selectedIds.Except(existingIds).ToList();
            
            var itemsToAdd = await _dbContext.Categories.Where(x => idsToAdd.Contains(x.Id)).ToListAsync();
            
            foreach (var itemToRemove in itemsToRemove)
            {
                dbEntity.BlogCategories.Remove(itemToRemove);
            }
            foreach (var itemToAdd in itemsToAdd)
            {
                dbEntity.BlogCategories.Add(itemToAdd);
            }
        }
    }
}
";

        public const string EntityValidator = @"using System.Collections.Generic;
using FluentValidation;
using RapidCMS.Example.ModelMaker.Validators;
using RapidCMS.ModelMaker.Validation;
using RapidCMS.ModelMaker.Validation.Config;

#nullable enable

namespace RapidCMS.ModelMaker
{
    public class BlogValidator : AbstractValidatorAdapter<Blog>
    {
        public BlogValidator()
        {
            RuleFor(x => x.Title)
                .MinimumLength(1)
                .MaximumLength(127)
                .BannedContent(new BannedContentValidationConfig { BannedWords = new List<string> { ""a"", ""b"", ""c"" } });
            RuleFor(x => x.IsPublished)
                .NotNull();
            RuleFor(x => x.PublishDate)
                .NotNull();
            RuleFor(x => x.MainCategoryId)
                .NotNull();
        }
    }
}
";
    }
}
