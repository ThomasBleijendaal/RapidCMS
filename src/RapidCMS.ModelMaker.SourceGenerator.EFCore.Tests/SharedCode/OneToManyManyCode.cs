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
                            section.AddField(x => x.OneId).SetType(typeof(RapidCMS.UI.Components.Editors.EntityPicker)).SetCollectionRelation(""one-to-many-ones"").SetName(""One"");
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
            builder.HasOne(x => x.One).WithMany(x => x.Relation).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
";
        public const string Entity = @"using RapidCMS.Core.Abstractions.Data;

#nullable enable

namespace RapidCMS.ModelMaker
{
    public partial class OnetoManyMany : IEntity
    {
        public int Id { get; set; }
        string? IEntity.Id { get => Id.ToString(); set => Id = int.Parse(value ?? ""0""); }
        
        public System.String Name { get; set; }
        
        public int? OneId { get; set; }
        public RapidCMS.ModelMaker.OnetoManyOne? One { get; set; }
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
                var entity = await _dbContext.OnetoManyManys.FirstOrDefaultAsync(x => x.Id == intId);
                if (entity != null)
                {
                    _dbContext.OnetoManyManys.Remove(entity);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }
        
        public override async Task<IEnumerable<OnetoManyMany>> GetAllAsync(IParent? parent, IQuery<OnetoManyMany> query)
        {
            return await query.ApplyOrder(query.ApplyDataView(_dbContext.OnetoManyManys)).Skip(query.Skip).Take(query.Take).AsNoTracking().ToListAsync();
        }
        
        public override async Task<OnetoManyMany?> GetByIdAsync(string id, IParent? parent)
        {
            if (int.TryParse(id, out var intId))
            {
                return await _dbContext.OnetoManyManys.AsNoTracking().FirstOrDefaultAsync(x => x.Id == intId);
            }
            return default;
        }
        
        public override async Task<OnetoManyMany?> InsertAsync(IEditContext<OnetoManyMany> editContext)
        {
            var entity = editContext.Entity;
            
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
            var entity = await _dbContext.OnetoManyManys.FirstAsync(x => x.Id == editContext.Entity.Id);
            
            entity.Name = editContext.Entity.Name;
            entity.OneId = editContext.Entity.OneId;
            
            await _dbContext.SaveChangesAsync();
        }
    }
}
";
    }
}
