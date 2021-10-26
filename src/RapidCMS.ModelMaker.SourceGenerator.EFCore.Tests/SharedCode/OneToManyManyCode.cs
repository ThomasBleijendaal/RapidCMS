namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests.SharedCode
{
    public static class OneToManyManyCode
    {
        public const string Collection = @"using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Providers;
using RapidCMS.Core.Repositories;

#nullable enable

namespace RapidCMS.ModelMaker
{
    public static class OnetoManyManyCollection
    {
        public static void AddOnetoManyManyCollection(this ICmsConfig config)
        {
            config.AddCollection<OnetoManyMany, BaseRepository<OnetoManyMany>>(
                ""one-to-many-manys"",
                ""Database"",
                ""Cyan30"",
                ""One to Many - Manys"",
                collection =>
                {
                    collection.SetTreeView(x => x.Name);
                    collection.SetElementConfiguration(x => x.Id, x => x.Name);
                    collection.AddEntityValidator<OnetoManyManyValidator>();
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
                            section.AddField(x => x.Many).SetType(typeof(RapidCMS.UI.Components.Editors.EntitiesPicker)).SetCollectionRelation(""one-to-many-ones"").SetName(""Many"");
                        });
                    });
                });
        }
    }
}
";
        public const string EntityTypeConfiguration = @"using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#nullable enable

namespace RapidCMS.ModelMaker
{
    public class OnetoManyManyConfiguration : IEntityTypeConfiguration<OnetoManyMany>
    {
        public void Configure(EntityTypeBuilder<OnetoManyMany> builder)
        {
            builder.HasMany(x => x.Many).WithOne(x => x.One).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
";
        public const string Entity = @"using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Data;

#nullable enable

namespace RapidCMS.ModelMaker
{
    public partial class OnetoManyMany : IEntity
    {
        public int Id { get; set; }
        string? IEntity.Id { get => Id.ToString(); set => Id = int.Parse(value ?? ""0""); }
        
        public System.String Name { get; set; }
        
        public ICollection<RapidCMS.ModelMaker.OnetoManyOne> Many { get; set; } = new List<RapidCMS.ModelMaker.OnetoManyOne>();
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
    public class OnetoManyManyRepository : BaseRepository<OnetoManyMany>
    {
        private readonly ModelMakerDbContext _dbContext;
        
        public OnetoManyManyRepository(ModelMakerDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public override async Task DeleteAsync(string id, IParent? parent)
        {
            if (int.TryParse(id, out var intId))
            {
                var entity = await _dbContext.OnetoManyManys.Include(x => x.Many).FirstOrDefaultAsync(x => x.Id == intId);
                if (entity != null)
                {
                    _dbContext.OnetoManyManys.Remove(entity);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }
        
        public override async Task<IEnumerable<OnetoManyMany>> GetAllAsync(IParent? parent, IView<OnetoManyMany> view)
        {
            return await view.ApplyOrder(view.ApplyDataView(_dbContext.OnetoManyManys)).Skip(view.Skip).Take(view.Take).AsNoTracking().ToListAsync();
        }
        
        public override async Task<OnetoManyMany?> GetByIdAsync(string id, IParent? parent)
        {
            if (int.TryParse(id, out var intId))
            {
                return await _dbContext.OnetoManyManys.Include(x => x.Many).AsNoTracking().FirstOrDefaultAsync(x => x.Id == intId);
            }
            return default;
        }
        
        public override async Task<OnetoManyMany?> InsertAsync(IEditContext<OnetoManyMany> editContext)
        {
            var entity = editContext.Entity;
            
            var relations = editContext.GetRelationContainer();
            await HandleManyAsync(entity, relations);
            
            var entry = _dbContext.OnetoManyManys.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entry.Entity;
        }
        
        public override Task<OnetoManyMany> NewAsync(IParent? parent, Type? variantType = null)
        {
            return Task.FromResult(new OnetoManyMany());
        }
        
        public override async Task UpdateAsync(IEditContext<OnetoManyMany> editContext)
        {
            var entity = await _dbContext.OnetoManyManys.Include(x => x.Many).FirstAsync(x => x.Id == editContext.Entity.Id);
            
            entity.Name = editContext.Entity.Name;
            
            var relations = editContext.GetRelationContainer();
            await HandleManyAsync(entity, relations);
            
            await _dbContext.SaveChangesAsync();
        }
        
        private async Task HandleManyAsync(OnetoManyMany dbEntity, IRelationContainer relations)
        {
            var selectedIds = relations.GetRelatedElementIdsFor<OnetoManyMany, ICollection<RapidCMS.ModelMaker.OnetoManyOne>, int>(x => x.Many) ?? Enumerable.Empty<int>();
            var existingIds = dbEntity.Many.Select(x => x.Id);
            
            var itemsToRemove = dbEntity.Many.Where(x => !selectedIds.Contains(x.Id)).ToList();
            var idsToAdd = selectedIds.Except(existingIds).ToList();
            
            var itemsToAdd = await _dbContext.OnetoManyOnes.Where(x => idsToAdd.Contains(x.Id)).ToListAsync();
            
            foreach (var itemToRemove in itemsToRemove)
            {
                dbEntity.Many.Remove(itemToRemove);
            }
            foreach (var itemToAdd in itemsToAdd)
            {
                dbEntity.Many.Add(itemToAdd);
            }
        }
    }
}
";
    }
}
