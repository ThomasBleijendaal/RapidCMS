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
                    .UseSqlServer("Server=ICAN-SEN\\SQLEX2019;Database=ModelMaker;user id=sa;password=lostman@123;MultipleActiveResultSets=true;")
                    .Options);
    }
}
