namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests.SharedCode
{
    public static class ManyToManyManyACode
    {
        public const string Collection = @"using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Providers;
using RapidCMS.Core.Repositories;

#nullable enable

namespace RapidCMS.ModelMaker
{
    public static class ManytoManyManyACollection
    {
        public static void AddManytoManyManyACollection(this ICmsConfig config)
        {
            config.AddCollection<ManytoManyManyA, BaseRepository<ManytoManyManyA>>(
                ""many-to-many-many-as"",
                ""Database"",
                ""PinkRed10"",
                ""Many to Many - Many As"",
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
                            section.AddField(x => x.Bs).SetType(typeof(RapidCMS.UI.Components.Editors.EntitiesPicker)).SetCollectionRelation(""many-to-many-many-bs"").SetName(""Bs"");
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
    public class ManytoManyManyAConfiguration : IEntityTypeConfiguration<ManytoManyManyA>
    {
        public void Configure(EntityTypeBuilder<ManytoManyManyA> builder)
        {
            builder.HasMany(x => x.Bs).WithMany(x => x.As);
        }
    }
}
";
        public const string Entity = @"using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Data;

#nullable enable

namespace RapidCMS.ModelMaker
{
    public partial class ManytoManyManyA : IEntity
    {
        public int Id { get; set; }
        string? IEntity.Id { get => Id.ToString(); set => Id = int.Parse(value ?? ""0""); }
        
        public System.String Name { get; set; }
        
        public ICollection<RapidCMS.ModelMaker.ManytoManyManyB> Bs { get; set; } = new List<RapidCMS.ModelMaker.ManytoManyManyB>();
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
    public class ManytoManyManyARepository : BaseRepository<ManytoManyManyA>
    {
        private readonly ModelMakerDbContext _dbContext;
        
        public ManytoManyManyARepository(ModelMakerDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public override async Task DeleteAsync(string id, IParent? parent)
        {
            if (int.TryParse(id, out var intId))
            {
                var entity = await _dbContext.ManytoManyManyAs.Include(x => x.Bs).FirstOrDefaultAsync(x => x.Id == intId);
                if (entity != null)
                {
                    _dbContext.ManytoManyManyAs.Remove(entity);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }
        
        public override async Task<IEnumerable<ManytoManyManyA>> GetAllAsync(IParent? parent, IQuery<ManytoManyManyA> query)
        {
            return await query.ApplyOrder(query.ApplyDataView(_dbContext.ManytoManyManyAs)).Skip(query.Skip).Take(query.Take).AsNoTracking().ToListAsync();
        }
        
        public override async Task<ManytoManyManyA?> GetByIdAsync(string id, IParent? parent)
        {
            if (int.TryParse(id, out var intId))
            {
                return await _dbContext.ManytoManyManyAs.Include(x => x.Bs).AsNoTracking().FirstOrDefaultAsync(x => x.Id == intId);
            }
            return default;
        }
        
        public override async Task<ManytoManyManyA?> InsertAsync(IEditContext<ManytoManyManyA> editContext)
        {
            var entity = editContext.Entity;
            
            var relations = editContext.GetRelationContainer();
            await HandleBsAsync(entity, relations);
            
            var entry = _dbContext.ManytoManyManyAs.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entry.Entity;
        }
        
        public override Task<ManytoManyManyA> NewAsync(IParent? parent, Type? variantType = null)
        {
            return Task.FromResult(new ManytoManyManyA());
        }
        
        public override async Task UpdateAsync(IEditContext<ManytoManyManyA> editContext)
        {
            var entity = await _dbContext.ManytoManyManyAs.Include(x => x.Bs).FirstAsync(x => x.Id == editContext.Entity.Id);
            
            entity.Name = editContext.Entity.Name;
            
            var relations = editContext.GetRelationContainer();
            await HandleBsAsync(entity, relations);
            
            await _dbContext.SaveChangesAsync();
        }
        
        private async Task HandleBsAsync(ManytoManyManyA dbEntity, IRelationContainer relations)
        {
            var selectedIds = relations.GetRelatedElementIdsFor<ManytoManyManyA, ICollection<RapidCMS.ModelMaker.ManytoManyManyB>, int>(x => x.Bs) ?? Enumerable.Empty<int>();
            var existingIds = dbEntity.Bs.Select(x => x.Id);
            
            var itemsToRemove = dbEntity.Bs.Where(x => !selectedIds.Contains(x.Id)).ToList();
            var idsToAdd = selectedIds.Except(existingIds).ToList();
            
            var itemsToAdd = await _dbContext.ManytoManyManyBs.Where(x => idsToAdd.Contains(x.Id)).ToListAsync();
            
            foreach (var itemToRemove in itemsToRemove)
            {
                dbEntity.Bs.Remove(itemToRemove);
            }
            foreach (var itemToAdd in itemsToAdd)
            {
                dbEntity.Bs.Add(itemToAdd);
            }
        }
    }
}
";
    }
}
