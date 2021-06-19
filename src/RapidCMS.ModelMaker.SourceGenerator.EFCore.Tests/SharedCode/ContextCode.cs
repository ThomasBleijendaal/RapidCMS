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
            
            modelBuilder.ApplyConfiguration(new BlogConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        }
        
        public DbSet<Blog> Blogs { get; set; } = default!;
        
        public DbSet<Category> Categories { get; set; } = default!;
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
    }
}
