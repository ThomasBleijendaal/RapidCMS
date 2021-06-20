namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests.SharedCode
{
    public static class OneToOneOneBCode
    {
        public const string Collection = @"using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Providers;
using RapidCMS.Core.Repositories;

#nullable enable

namespace RapidCMS.ModelMaker
{
    public static class OnetoOneOneBCollection
    {
        public static void AddOnetoOneOneBCollection(this ICmsConfig config)
        {
            config.AddCollection<OnetoOneOneB, BaseRepository<OnetoOneOneB>>(
                ""one-to-one-one-bs"",
                ""Database"",
                ""Orange10"",
                ""One to One - One Bs"",
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
                            section.AddField(x => x.A == null ? 0 : x.A.Id).DisableWhen((e, s) => true).SetType(typeof(RapidCMS.UI.Components.Editors.EntityPicker)).SetCollectionRelation(""one-to-one-one-as"").SetName(""A"");
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
    public class OnetoOneOneBConfiguration : IEntityTypeConfiguration<OnetoOneOneB>
    {
        public void Configure(EntityTypeBuilder<OnetoOneOneB> builder)
        {
            builder.HasOne(x => x.A).WithOne(x => x.B).HasForeignKey<RapidCMS.ModelMaker.OnetoOneOneA>(x => x.BId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
";
        public const string Entity = @"using RapidCMS.Core.Abstractions.Data;

#nullable enable

namespace RapidCMS.ModelMaker
{
    public partial class OnetoOneOneB : IEntity
    {
        public int Id { get; set; }
        string? IEntity.Id { get => Id.ToString(); set => Id = int.Parse(value ?? ""0""); }
        
        public System.String Name { get; set; }
        
        public RapidCMS.ModelMaker.OnetoOneOneA? A { get; set; }
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
    public class OnetoOneOneBRepository : BaseRepository<OnetoOneOneB>
    {
        private readonly ModelMakerDbContext _dbContext;
        
        public OnetoOneOneBRepository(ModelMakerDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public override async Task DeleteAsync(string id, IParent? parent)
        {
            if (int.TryParse(id, out var intId))
            {
                var entity = await _dbContext.OnetoOneOneBs.Include(x => x.A).FirstOrDefaultAsync(x => x.Id == intId);
                if (entity != null)
                {
                    _dbContext.OnetoOneOneBs.Remove(entity);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }
        
        public override async Task<IEnumerable<OnetoOneOneB>> GetAllAsync(IParent? parent, IQuery<OnetoOneOneB> query)
        {
            return await query.ApplyOrder(query.ApplyDataView(_dbContext.OnetoOneOneBs)).Skip(query.Skip).Take(query.Take).AsNoTracking().ToListAsync();
        }
        
        public override async Task<OnetoOneOneB?> GetByIdAsync(string id, IParent? parent)
        {
            if (int.TryParse(id, out var intId))
            {
                return await _dbContext.OnetoOneOneBs.Include(x => x.A).AsNoTracking().FirstOrDefaultAsync(x => x.Id == intId);
            }
            return default;
        }
        
        public override async Task<OnetoOneOneB?> InsertAsync(IEditContext<OnetoOneOneB> editContext)
        {
            var entity = editContext.Entity;
            
            var entry = _dbContext.OnetoOneOneBs.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entry.Entity;
        }
        
        public override Task<OnetoOneOneB> NewAsync(IParent? parent, Type? variantType = null)
        {
            return Task.FromResult(new OnetoOneOneB());
        }
        
        public override async Task UpdateAsync(IEditContext<OnetoOneOneB> editContext)
        {
            var entity = await _dbContext.OnetoOneOneBs.Include(x => x.A).FirstAsync(x => x.Id == editContext.Entity.Id);
            
            entity.Name = editContext.Entity.Name;
            
            await _dbContext.SaveChangesAsync();
        }
    }
}
";
    }
}
