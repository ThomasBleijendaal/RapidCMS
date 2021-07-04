using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using RapidCMS.ModelMaker;

namespace RapidCMS.Example.ModelMaker.Models
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ModelMakerDbContext>
    {
        public ModelMakerDbContext CreateDbContext(string[] args) 
            => new ModelMakerDbContext(
                new DbContextOptionsBuilder<ModelMakerDbContext>()
                    .UseSqlServer("server=localhost\\sqlexpress;database=ModelMaker;integrated security=true;")
                    .Options);
    }
}
