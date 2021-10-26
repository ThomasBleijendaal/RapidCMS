namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests.SharedCode
{
    public static class OneToManyOneCode
    {
        public const string Collection = @"using RapidCMS.Core.Abstractions.Config;
using RapidCMS.Core.Enums;
using RapidCMS.Core.Extensions;
using RapidCMS.Core.Providers;
using RapidCMS.Core.Repositories;

#nullable enable

namespace RapidCMS.ModelMaker
{
    public static class OnetoManyOneCollection
    {
        public static void AddOnetoManyOneCollection(this ICmsConfig config)
        {
            config.AddCollection<OnetoManyOne, BaseRepository<OnetoManyOne>>(
                ""one-to-many-ones"",
                ""Database"",
                ""Cyan30"",
                ""One to Many - Ones"",
                collection =>
                {
                    collection.SetTreeView(x => x.Name);
                    collection.SetElementConfiguration(x => x.Id, x => x.Name);
                    collection.AddEntityValidator<OnetoManyOneValidator>();
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
                            section.AddField(x => x.OneId).SetType(typeof(RapidCMS.UI.Components.Editors.EntityPicker)).SetCollectionRelation(""one-to-many-manys"").SetName(""One"");
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
    public class OnetoManyOneConfiguration : IEntityTypeConfiguration<OnetoManyOne>
    {
        public void Configure(EntityTypeBuilder<OnetoManyOne> builder)
        {
            builder.HasOne(x => x.One).WithMany(x => x.Many).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
";
        public const string Entity = @"using RapidCMS.Core.Abstractions.Data;

#nullable enable

namespace RapidCMS.ModelMaker
{
    public partial class OnetoManyOne : IEntity
    {
        public int Id { get; set; }
        string? IEntity.Id { get => Id.ToString(); set => Id = int.Parse(value ?? ""0""); }
        
        public System.String Name { get; set; }
        
        public int? OneId { get; set; }
        public RapidCMS.ModelMaker.OnetoManyMany? One { get; set; }
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
    public class OnetoManyOneRepository : BaseRepository<OnetoManyOne>
    {
        private readonly ModelMakerDbContext _dbContext;
        
        public OnetoManyOneRepository(ModelMakerDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public override async Task DeleteAsync(string id, IParent? parent)
        {
            if (int.TryParse(id, out var intId))
            {
                var entity = await _dbContext.OnetoManyOnes.FirstOrDefaultAsync(x => x.Id == intId);
                if (entity != null)
                {
                    _dbContext.OnetoManyOnes.Remove(entity);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }
        
        public override async Task<IEnumerable<OnetoManyOne>> GetAllAsync(IParent? parent, IView<OnetoManyOne> view)
        {
            return await view.ApplyOrder(view.ApplyDataView(_dbContext.OnetoManyOnes)).Skip(view.Skip).Take(view.Take).AsNoTracking().ToListAsync();
        }
        
        public override async Task<OnetoManyOne?> GetByIdAsync(string id, IParent? parent)
        {
            if (int.TryParse(id, out var intId))
            {
                return await _dbContext.OnetoManyOnes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == intId);
            }
            return default;
        }
        
        public override async Task<OnetoManyOne?> InsertAsync(IEditContext<OnetoManyOne> editContext)
        {
            var entity = editContext.Entity;
            
            var entry = _dbContext.OnetoManyOnes.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entry.Entity;
        }
        
        public override Task<OnetoManyOne> NewAsync(IParent? parent, Type? variantType = null)
        {
            return Task.FromResult(new OnetoManyOne());
        }
        
        public override async Task UpdateAsync(IEditContext<OnetoManyOne> editContext)
        {
            var entity = await _dbContext.OnetoManyOnes.FirstAsync(x => x.Id == editContext.Entity.Id);
            
            entity.Name = editContext.Entity.Name;
            entity.OneId = editContext.Entity.OneId;
            
            await _dbContext.SaveChangesAsync();
        }
    }
}
";
    }
}
