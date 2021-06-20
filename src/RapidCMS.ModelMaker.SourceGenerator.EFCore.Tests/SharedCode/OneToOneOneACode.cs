namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests.SharedCode
{
    public static class OneToOneOneACode
    {
        public const string Collection = @"using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Providers;
using RapidCMS.Core.Repositories;

#nullable enable

namespace RapidCMS.ModelMaker
{
    public static class OnetoOneOneACollection
    {
        public static void AddOnetoOneOneACollection(this ICmsConfig config)
        {
            config.AddCollection<OnetoOneOneA, BaseRepository<OnetoOneOneA>>(
                ""one-to-one-one-as"",
                ""Database"",
                ""Orange10"",
                ""One to One - One As"",
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
                            section.AddField(x => x.BId).SetType(typeof(RapidCMS.UI.Components.Editors.EntityPicker)).SetCollectionRelation(""one-to-one-one-bs"").SetName(""B"");
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
    public class OnetoOneOneAConfiguration : IEntityTypeConfiguration<OnetoOneOneA>
    {
        public void Configure(EntityTypeBuilder<OnetoOneOneA> builder)
        {
        }
    }
}
";
        public const string Entity = @"using RapidCMS.Core.Abstractions.Data;

#nullable enable

namespace RapidCMS.ModelMaker
{
    public partial class OnetoOneOneA : IEntity
    {
        public int Id { get; set; }
        string? IEntity.Id { get => Id.ToString(); set => Id = int.Parse(value ?? ""0""); }
        
        public System.String Name { get; set; }
        
        public int? BId { get; set; }
        public RapidCMS.ModelMaker.OnetoOneOneB? B { get; set; }
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
    public class OnetoOneOneARepository : BaseRepository<OnetoOneOneA>
    {
        private readonly ModelMakerDbContext _dbContext;
        
        public OnetoOneOneARepository(ModelMakerDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public override async Task DeleteAsync(string id, IParent? parent)
        {
            if (int.TryParse(id, out var intId))
            {
                var entity = await _dbContext.OnetoOneOneAs.FirstOrDefaultAsync(x => x.Id == intId);
                if (entity != null)
                {
                    _dbContext.OnetoOneOneAs.Remove(entity);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }
        
        public override async Task<IEnumerable<OnetoOneOneA>> GetAllAsync(IParent? parent, IQuery<OnetoOneOneA> query)
        {
            return await query.ApplyOrder(query.ApplyDataView(_dbContext.OnetoOneOneAs)).Skip(query.Skip).Take(query.Take).AsNoTracking().ToListAsync();
        }
        
        public override async Task<OnetoOneOneA?> GetByIdAsync(string id, IParent? parent)
        {
            if (int.TryParse(id, out var intId))
            {
                return await _dbContext.OnetoOneOneAs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == intId);
            }
            return default;
        }
        
        public override async Task<OnetoOneOneA?> InsertAsync(IEditContext<OnetoOneOneA> editContext)
        {
            var entity = editContext.Entity;
            
            var entry = _dbContext.OnetoOneOneAs.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entry.Entity;
        }
        
        public override Task<OnetoOneOneA> NewAsync(IParent? parent, Type? variantType = null)
        {
            return Task.FromResult(new OnetoOneOneA());
        }
        
        public override async Task UpdateAsync(IEditContext<OnetoOneOneA> editContext)
        {
            var entity = await _dbContext.OnetoOneOneAs.FirstAsync(x => x.Id == editContext.Entity.Id);
            
            entity.BId = editContext.Entity.BId;
            entity.Name = editContext.Entity.Name;
            
            await _dbContext.SaveChangesAsync();
        }
    }
}
";
    }
}
