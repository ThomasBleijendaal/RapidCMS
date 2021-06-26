namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests.SharedCode
{
    public static class ManyToManyManyBCode
    {
        public const string Collection = @"using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Providers;
using RapidCMS.Core.Repositories;

#nullable enable

namespace RapidCMS.ModelMaker
{
    public static class ManytoManyManyBCollection
    {
        public static void AddManytoManyManyBCollection(this ICmsConfig config)
        {
            config.AddCollection<ManytoManyManyB, BaseRepository<ManytoManyManyB>>(
                ""many-to-many-many-bs"",
                ""Database"",
                ""PinkRed10"",
                ""Many to Many - Many Bs"",
                collection =>
                {
                    collection.SetTreeView(x => x.Name);
                    collection.SetElementConfiguration(x => x.Id, x => x.Name);
                    collection.AddEntityValidator<ManytoManyManyBValidator>();
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
                            section.AddField(x => x.As).SetType(typeof(RapidCMS.UI.Components.Editors.EntitiesPicker)).SetCollectionRelation(""many-to-many-many-as"").SetName(""As"");
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
    public class ManytoManyManyBConfiguration : IEntityTypeConfiguration<ManytoManyManyB>
    {
        public void Configure(EntityTypeBuilder<ManytoManyManyB> builder)
        {
            builder.HasMany(x => x.As).WithMany(x => x.Bs);
        }
    }
}
";
        public const string Entity = @"using System.Collections.Generic;
using RapidCMS.Core.Abstractions.Data;

#nullable enable

namespace RapidCMS.ModelMaker
{
    public partial class ManytoManyManyB : IEntity
    {
        public int Id { get; set; }
        string? IEntity.Id { get => Id.ToString(); set => Id = int.Parse(value ?? ""0""); }
        
        public System.String Name { get; set; }
        
        public ICollection<RapidCMS.ModelMaker.ManytoManyManyA> As { get; set; } = new List<RapidCMS.ModelMaker.ManytoManyManyA>();
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
    public class ManytoManyManyBRepository : BaseRepository<ManytoManyManyB>
    {
        private readonly ModelMakerDbContext _dbContext;
        
        public ManytoManyManyBRepository(ModelMakerDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public override async Task DeleteAsync(string id, IParent? parent)
        {
            if (int.TryParse(id, out var intId))
            {
                var entity = await _dbContext.ManytoManyManyBs.Include(x => x.As).FirstOrDefaultAsync(x => x.Id == intId);
                if (entity != null)
                {
                    _dbContext.ManytoManyManyBs.Remove(entity);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }
        
        public override async Task<IEnumerable<ManytoManyManyB>> GetAllAsync(IParent? parent, IQuery<ManytoManyManyB> query)
        {
            return await query.ApplyOrder(query.ApplyDataView(_dbContext.ManytoManyManyBs)).Skip(query.Skip).Take(query.Take).AsNoTracking().ToListAsync();
        }
        
        public override async Task<ManytoManyManyB?> GetByIdAsync(string id, IParent? parent)
        {
            if (int.TryParse(id, out var intId))
            {
                return await _dbContext.ManytoManyManyBs.Include(x => x.As).AsNoTracking().FirstOrDefaultAsync(x => x.Id == intId);
            }
            return default;
        }
        
        public override async Task<ManytoManyManyB?> InsertAsync(IEditContext<ManytoManyManyB> editContext)
        {
            var entity = editContext.Entity;
            
            var relations = editContext.GetRelationContainer();
            await HandleAsAsync(entity, relations);
            
            var entry = _dbContext.ManytoManyManyBs.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entry.Entity;
        }
        
        public override Task<ManytoManyManyB> NewAsync(IParent? parent, Type? variantType = null)
        {
            return Task.FromResult(new ManytoManyManyB());
        }
        
        public override async Task UpdateAsync(IEditContext<ManytoManyManyB> editContext)
        {
            var entity = await _dbContext.ManytoManyManyBs.Include(x => x.As).FirstAsync(x => x.Id == editContext.Entity.Id);
            
            entity.Name = editContext.Entity.Name;
            
            var relations = editContext.GetRelationContainer();
            await HandleAsAsync(entity, relations);
            
            await _dbContext.SaveChangesAsync();
        }
        
        private async Task HandleAsAsync(ManytoManyManyB dbEntity, IRelationContainer relations)
        {
            var selectedIds = relations.GetRelatedElementIdsFor<ManytoManyManyB, ICollection<RapidCMS.ModelMaker.ManytoManyManyA>, int>(x => x.As) ?? Enumerable.Empty<int>();
            var existingIds = dbEntity.As.Select(x => x.Id);
            
            var itemsToRemove = dbEntity.As.Where(x => !selectedIds.Contains(x.Id)).ToList();
            var idsToAdd = selectedIds.Except(existingIds).ToList();
            
            var itemsToAdd = await _dbContext.ManytoManyManyAs.Where(x => idsToAdd.Contains(x.Id)).ToListAsync();
            
            foreach (var itemToRemove in itemsToRemove)
            {
                dbEntity.As.Remove(itemToRemove);
            }
            foreach (var itemToAdd in itemsToAdd)
            {
                dbEntity.As.Add(itemToAdd);
            }
        }
    }
}
";
    }
}
