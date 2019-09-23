using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TestLibrary.Entities;

namespace TestLibrary.Data
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CountryEntity>()
                .HasKey(x => x._Id);

            modelBuilder.Entity<PersonEntity>()
                .HasKey(x => x._Id);

            modelBuilder.Entity<PersonCountryEntity>()
                .HasKey(t => new { t.CountryId, t.PersonId });
        }

        public DbSet<CountryEntity> Countries { get; set; }
        public DbSet<PersonEntity> Persons { get; set; }
        public DbSet<PersonCountryEntity> PersonContries { get; set; }
    }

    public class TestDbContextFactory : IDesignTimeDbContextFactory<TestDbContext>
    {
        public TestDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TestDbContext>();
            optionsBuilder.UseSqlServer(@"Server=localhost\SQLEXPRESS;Database=RapidCMS;Trusted_Connection=True;");

            var context = new TestDbContext(optionsBuilder.Options);

            return context;
        }
    }
}
