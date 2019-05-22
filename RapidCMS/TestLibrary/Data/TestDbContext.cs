using Microsoft.EntityFrameworkCore;
using TestLibrary.Entities;

namespace TestLibrary.Data
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<CountryEntity> Countries { get; set; }
        public DbSet<PersonEntity> Persons { get; set; }
        public DbSet<PersonCountryEntity> PersonContries { get; set; }
    }
}
