namespace RapidCMS.ModelMaker.SourceGenerator.EFCore.Tests.SharedCode
{
    public static class ContextCode
    {
        public const string BlogCategoryContext = @"using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#nullable enable

namespace RapidCMS.ModelMaker
{
    public partial class ModelMakerDbContext : DbContext
    {
        public ModelMakerDbContext(DbContextOptions options) : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new BlogConfiguration());
        }
        
        public DbSet<Category> Categories { get; set; } = default!;
        
        public DbSet<Blog> Blogs { get; set; } = default!;
    }
}
";

        public const string CategoryContext = @"using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#nullable enable

namespace RapidCMS.ModelMaker
{
    public partial class ModelMakerDbContext : DbContext
    {
        public ModelMakerDbContext(DbContextOptions options) : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        }
        
        public DbSet<Category> Categories { get; set; } = default!;
    }
}
";

        public const string OneToManyContext = @"using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

#nullable enable

namespace RapidCMS.ModelMaker
{
    public partial class ModelMakerDbContext : DbContext
    {
        public ModelMakerDbContext(DbContextOptions options) : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.ApplyConfiguration(new OnetoManyManyConfiguration());
            modelBuilder.ApplyConfiguration(new OnetoManyOneConfiguration());
        }
        
        public DbSet<OnetoManyMany> OnetoManyManys { get; set; } = default!;
        
        public DbSet<OnetoManyOne> OnetoManyOnes { get; set; } = default!;
    }
}
";
    }
}
